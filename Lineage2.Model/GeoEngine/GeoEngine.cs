using Lineage2.Model.Configs;
using Lineage2.Model.GeoEngine.GeoData;
using Lineage2.Model.GeoEngine.PathFinding;
using Lineage2.Model.Locations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lineage2.Model.GeoEngine
{
    public class GeoEngine
    {
        private readonly ILogger logger = Log.Logger.ForContext<GeoEngine>();

        private static string GEO_BUG = "%d;%d;%d;%d;%d;%d;%d;%s\r\n";

        private ABlock[,] _blocks;
        private BlockNull _nullBlock;

        //private Set<ItemInstance> _debugItems = ConcurrentHashMap.newKeySet();

        // pre-allocated buffers
        private BufferHolder[] _buffers;

        // pathfinding statistics
        private int _findSuccess = 0;
        private int _findFails = 0;
        private int _postFilterPlayableUses = 0;
        private int _postFilterUses = 0;
        private long _postFilterElapsed = 0;

        GeodataConfig geodataConfig;

        /**
         * GeoEngine contructor. Loads all geodata files of chosen geodata format.
         */
        public GeoEngine(GeodataConfig geodataConfig)
        {
            this.geodataConfig = geodataConfig;
            // initialize block container
            _blocks = new ABlock[GeoStructure.GeoBlocksX, GeoStructure.GeoBlocksY];

            // load null block
            _nullBlock = new BlockNull();

            // initialize multilayer temporarily buffer
            BlockMultilayer.Initialize();

            // load geo files according to geoengine config setup
            ExProperties props = Config.initProperties(Config.GEOENGINE_FILE);
            int loaded = 0;
            int failed = 0;
            for (int rx = GeoStructure.TileXMin; rx <= GeoStructure.TileXMax; rx++)
            {
                for (int ry = GeoStructure.TileYMin; ry <= GeoStructure.TileYMax; ry++)
                {
                    if (props.containsKey(rx + "_" + ry))
                    {
                        // region file is load-able, try to load it
                        if (loadGeoBlocks(rx, ry))
                            loaded++;
                        else
                            failed++;
                    }
                    else
                    {
                        // region file is not load-able, load null blocks
                        loadNullBlocks(rx, ry);
                    }
                }
            }

            logger.Information("Загружено{0} L2D region файлов.", loaded);

            // release multilayer block temporarily buffer
            BlockMultilayer.Release();

            if (failed > 0)
            {
                logger.Fatal("Не удалось загрузить файлы региона L2D {0}. Пожалуйста, проверьте настройки \"geodata.properties\" и расположение файлов геоданных.", failed);
                //System.exit(1);
            }

            string[] array = geodataConfig.PathFindBuffers.Split(";");
            _buffers = new BufferHolder[array.Length];

            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                string buf = array[i];
                string[] args = buf.Split("x");

                try
                {
                    int size = int.Parse(args[1]);
                    count += size;
                    _buffers[i] = new BufferHolder(int.Parse(args[0]), size, geodataConfig);
                }
                catch (Exception e)
                {
                    logger.Error("Couldn't load buffer setting:", e);
                }
            }

            logger.Information("Loaded {0} node buffers.", count);
        }

        /**
         * Create list of node locations as result of calculated buffer node tree.
         * @param tarGet : the entry point
         * @return List<NodeLoc> : list of node location
         */
        private static LinkedList<Location> ConstructPath(Node tarGet)
        {
            // create empty list
            LinkedList<Location> list = new LinkedList<Location>();

            // set direction X/Y
            int dx = 0;
            int dy = 0;

            // Get tarGet parent
            Node parent = tarGet.GetParent();

            // while parent exists
            while (parent != null)
            {
                // Get parent <> tarGet direction X/Y
                int nx = parent.GetLoc().GetGeoX() - tarGet.GetLoc().GetGeoX();
                int ny = parent.GetLoc().GetGeoY() - tarGet.GetLoc().GetGeoY();

                // direction has changed?
                if (dx != nx || dy != ny)
                {
                    // add node to the beginning of the list
                    list.AddFirst(tarGet.GetLoc());

                    // update direction X/Y
                    dx = nx;
                    dy = ny;
                }

                // move to next node, set tarGet and Get its parent
                tarGet = parent;
                parent = tarGet.GetParent();
            }

            // return list
            return list;
        }

        /**
         * Provides optimize selection of the buffer. When all pre-initialized buffer are locked, creates new buffer and log this situation.
         * @param size : pre-calculated minimal required size
         * @param playable : moving object is playable?
         * @return NodeBuffer : buffer
         */
        private NodeBuffer GetBuffer(int size, bool playable)
        {
            NodeBuffer current = null;
            foreach (var holder in _buffers)
            {
                // Find proper size of buffer
                if (holder._size < size)
                    continue;

                // Find unlocked NodeBuffer
                foreach (NodeBuffer buffer in holder._buffer)
                {
                    if (!buffer.isLocked())
                        continue;

                    holder._uses++;
                    if (playable)
                        holder._playableUses++;

                    holder._elapsed += buffer.GetElapsedTime();
                    return buffer;
                }

                // NodeBuffer not found, allocate temporary buffer
                current = new NodeBuffer(holder._size, geodataConfig);
                current.isLocked();

                holder._overflows++;
                if (playable)
                    holder._playableOverflows++;
            }

            return current;
        }

        /**
         * Loads geodata from a file. When file does not exist, is corrupted or not consistent, loads none geodata.
         * @param regionX : Geodata file region X coordinate.
         * @param regionY : Geodata file region Y coordinate.
         * @return bool : True, when geodata file was loaded without problem.
         */
        private bool loadGeoBlocks(int regionX, int regionY)
        {
            string filename = string.Format(GeoFormat.L2D.GetFilename(), regionX, regionY);
            string filepath = geodataConfig.GeoDataPath + filename;

            // standard load
            try (RandomAccessFile raf = new RandomAccessFile(filepath, "r");
            FileChannel fc = raf.GetChannel())
{
                // initialize file buffer
                MappedByteBuffer buffer = fc.map(FileChannel.MapMode.READ_ONLY, 0, fc.size()).load();
                buffer.order(ByteOrder.LITTLE_ENDIAN);

                // Get block indexes
                int blockX = (regionX - GeoStructure.TileXMin) * GeoStructure.RegionBlocksX;
                int blockY = (regionY - GeoStructure.TileYMin) * GeoStructure.RegionBlocksY;

                // loop over region blocks
                for (int ix = 0; ix < GeoStructure.RegionBlocksX; ix++)
                {
                    for (int iy = 0; iy < GeoStructure.RegionBlocksY; iy++)
                    {
                        // Get block type
                        byte type = buffer.Get();

                        // load block according to block type
                        switch (type)
                        {
                            case GeoStructure.TypeFlatL2D:
                                _blocks[blockX + ix][blockY + iy] = new BlockFlat(buffer, GeoFormat.L2D);
                                break;

                            case GeoStructure.TypeComplexL2D:
                                _blocks[blockX + ix][blockY + iy] = new BlockComplex(buffer, GeoFormat.L2D);
                                break;

                            case GeoStructure.TypeMultilayerL2D:
                                _blocks[blockX + ix][blockY + iy] = new BlockMultilayer(buffer, GeoFormat.L2D);
                                break;

                            default:
                                throw new IllegalArgumentException("Unknown block type: " + type);
                        }
                    }
                }

                // check data consistency
                if (buffer.remaining() > 0)
                    logger.Warning("Region file {} can be corrupted, remaining {} bytes to read.", filename, buffer.remaining());

                // loading was successful
                return true;
            }

        catch (Exception e)
            {
                // an error occured while loading, load null blocks
                logger.Error("Error loading {} region file.", e, filename);

                // replace whole region file with null blocks
                loadNullBlocks(regionX, regionY);

                // loading was not successful
                return false;
            }
        }

        /**
         * Loads null blocks. Used when no region file is detected or an error occurs during loading.
         * @param regionX : Geodata file region X coordinate.
         * @param regionY : Geodata file region Y coordinate.
         */
        private void loadNullBlocks(int regionX, int regionY)
        {
            // Get block indexes
            int blockX = (regionX - GeoStructure.TileXMin) * GeoStructure.RegionBlocksX;
            int blockY = (regionY - GeoStructure.TileYMin) * GeoStructure.RegionBlocksY;

            // load all null blocks
            for (int ix = 0; ix < GeoStructure.RegionBlocksX; ix++)
                for (int iy = 0; iy < GeoStructure.RegionBlocksY; iy++)
                    _blocks[blockX + ix, blockY + iy] = _nullBlock;
        }

        /**
         * Returns the height of cell, which is closest to given coordinates.<br>
         * Geodata without {@link IGeoObject} are taken in consideration.
         * @param geoX : Cell geodata X coordinate.
         * @param geoY : Cell geodata Y coordinate.
         * @param worldZ : Cell world Z coordinate.
         * @return short : Cell geodata Z coordinate, closest to given coordinates.
         */
        private short GetHeightNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return GetBlock(geoX, geoY).GetHeightNearestOriginal(geoX, geoY, worldZ);
        }

        /**
         * Returns the NSWE flag byte of cell, which is closes to given coordinates.<br>
         * Geodata without {@link IGeoObject} are taken in consideration.
         * @param geoX : Cell geodata X coordinate.
         * @param geoY : Cell geodata Y coordinate.
         * @param worldZ : Cell world Z coordinate.
         * @return short : Cell NSWE flag byte coordinate, closest to given coordinates.
         */
        private byte GetNsweNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return GetBlock(geoX, geoY).GetNsweNearestOriginal(geoX, geoY, worldZ);
        }

        // GEODATA - GENERAL

        /**
         * Converts world X to geodata X.
         * @param worldX
         * @return int : Geo X
         */
        public static int GetGeoX(int worldX)
        {
            return (Math.Clamp(worldX, GeoStructure.WorldXMin, GeoStructure.WorldXMax) - GeoStructure.WorldXMin) >> 4;
        }

        /**
         * Converts world Y to geodata Y.
         * @param worldY
         * @return int : Geo Y
         */
        public static int GetGeoY(int worldY)
        {
            return (Math.Clamp(worldY, GeoStructure.WorldYMin, GeoStructure.WorldYMax) - GeoStructure.WorldYMin) >> 4;
        }

        /**
         * Converts geodata X to world X.
         * @param geoX
         * @return int : World X
         */
        public static int GetWorldX(int geoX)
        {
            return (Math.Clamp(geoX, 0, GeoStructure.GeoCellsX) << 4) + GeoStructure.WorldXMin + 8;
        }

        /**
         * Converts geodata Y to world Y.
         * @param geoY
         * @return int : World Y
         */
        public static int GetWorldY(int geoY)
        {
            return (Math.Clamp(geoY, 0, GeoStructure.GeoCellsY) << 4) + GeoStructure.WorldYMin + 8;
        }

        /**
         * Returns block of geodata on given coordinates.
         * @param geoX : Geodata X
         * @param geoY : Geodata Y
         * @return {@link ABlock} : Bloack of geodata.
         */
        public ABlock GetBlock(int geoX, int geoY)
        {
            return _blocks[geoX / GeoStructure.BlockCellsX, geoY / GeoStructure.BlockCellsY];
        }

        /**
         * Check if geo coordinates has geo.
         * @param geoX : Geodata X
         * @param geoY : Geodata Y
         * @return bool : True, if given geo coordinates have geodata
         */
        public bool hasGeoPos(int geoX, int geoY)
        {
            return GetBlock(geoX, geoY).HasGeoPos();
        }

        /**
         * Returns the height of cell, which is closest to given coordinates.
         * @param geoX : Cell geodata X coordinate.
         * @param geoY : Cell geodata Y coordinate.
         * @param worldZ : Cell world Z coordinate.
         * @return short : Cell geodata Z coordinate, closest to given coordinates.
         */
        public short GetHeightNearest(int geoX, int geoY, int worldZ)
        {
            return GetBlock(geoX, geoY).GetHeightNearest(geoX, geoY, worldZ);
        }

        /**
         * Returns the NSWE flag byte of cell, which is closes to given coordinates.
         * @param geoX : Cell geodata X coordinate.
         * @param geoY : Cell geodata Y coordinate.
         * @param worldZ : Cell world Z coordinate.
         * @return short : Cell NSWE flag byte coordinate, closest to given coordinates.
         */
        public byte GetNsweNearest(int geoX, int geoY, int worldZ)
        {
            return GetBlock(geoX, geoY).GetNsweNearest(geoX, geoY, worldZ);
        }

        /**
         * Check if world coordinates has geo.
         * @param worldX : World X
         * @param worldY : World Y
         * @return bool : True, if given world coordinates have geodata
         */
        public bool hasGeo(int worldX, int worldY)
        {
            return hasGeoPos(GetGeoX(worldX), GetGeoY(worldY));
        }

        /**
         * Returns closest Z coordinate according to geodata.
         * @param worldX : world x
         * @param worldY : world y
         * @param worldZ : world z
         * @return short : nearest Z coordinates according to geodata
         */
        public short GetHeight(int worldX, int worldY, int worldZ)
        {
            return GetHeightNearest(GetGeoX(worldX), GetGeoY(worldY), worldZ);
        }

        // GEODATA - DYNAMIC

        /**
         * Returns calculated NSWE flag byte as a description of {@link IGeoObject}.<br>
         * The {@link IGeoObject} is defined by bool 2D array, saying if the object is present on given cell or not.
         * @param inside : 2D description of {@link IGeoObject}
         * @return byte[][] : Returns NSWE flags of {@link IGeoObject}.
         */
        public static byte[][] calculateGeoObject(bool[][] inside)
        {
            // Get dimensions
            int width = inside.Length;
            int height = inside[0].Length;

            // create object flags for geodata, according to the geo object 2D description
            byte[][] result = new byte[width][height];

            // loop over each cell of the geo object
            for (int ix = 0; ix < width; ix++)
                for (int iy = 0; iy < height; iy++)
                    if (inside[ix][iy])
                    {
                        // cell is inside geo object, block whole movement (nswe = 0)
                        result[ix][iy] = 0;
                    }
                    else
                    {
                        // cell is outside of geo object, block only movement leading inside geo object

                        // set initial value -> no geodata change
                        byte nswe = (byte)0xFF;

                        // perform axial and diagonal checks
                        if (iy < height - 1)
                            if (inside[ix][iy + 1])
                                nswe &= ~GeoStructure.CellFlagS;
                        if (iy > 0)
                            if (inside[ix][iy - 1])
                                nswe &= ~GeoStructure.CellFlagN;
                        if (ix < width - 1)
                            if (inside[ix + 1][iy])
                                nswe &= ~GeoStructure.CellFlagE;
                        if (ix > 0)
                            if (inside[ix - 1][iy])
                                nswe &= ~GeoStructure.CellFlagW;
                        if (ix < (width - 1) && iy < (height - 1))
                            if (inside[ix + 1][iy + 1] || inside[ix][iy + 1] || inside[ix + 1][iy])
                                nswe &= ~GeoStructure.CellFlagSE;
                        if (ix < (width - 1) && iy > 0)
                            if (inside[ix + 1][iy - 1] || inside[ix][iy - 1] || inside[ix + 1][iy])
                                nswe &= ~GeoStructure.CellFlagNE;
                        if (ix > 0 && iy < (height - 1))
                            if (inside[ix - 1][iy + 1] || inside[ix][iy + 1] || inside[ix - 1][iy])
                                nswe &= ~GeoStructure.CellFlagSW;
                        if (ix > 0 && iy > 0)
                            if (inside[ix - 1][iy - 1] || inside[ix][iy - 1] || inside[ix - 1][iy])
                                nswe &= ~GeoStructure.CellFlagNW;

                        result[ix][iy] = nswe;
                    }

            return result;
        }

        /**
         * Add {@link IGeoObject} to the geodata.
         * @param object : An object using {@link IGeoObject} interface.
         */
        public void addGeoObject(IGeoObject geoObject)
        {
            toggleGeoObject(geoObject, true);
        }

        /**
         * Remove {@link IGeoObject} from the geodata.
         * @param object : An object using {@link IGeoObject} interface.
         */
        public void removeGeoObject(IGeoObject geoObject)
        {
            toggleGeoObject(geoObject, false);
        }

        /**
         * Toggles an {@link IGeoObject} in the geodata.
         * @param object : An object using {@link IGeoObject} interface.
         * @param add : Add/remove object.
         */
        private void toggleGeoObject(IGeoObject geoObject, bool add)
        {
            // Get object geo coordinates and data
            int minGX = geoObject.GetGeoX();
            int minGY = geoObject.GetGeoY();
            byte[][] geoData = geoObject.GetObjectGeoData();

            // Get min/max block coordinates
            int minBX = minGX / GeoStructure.BlockCellsX;
            int maxBX = (minGX + geoData.Length - 1) / GeoStructure.BlockCellsX;
            int minBY = minGY / GeoStructure.BlockCellsY;
            int maxBY = (minGY + geoData[0].Length - 1) / GeoStructure.BlockCellsY;

            // loop over affected blocks in X direction
            for (int bx = minBX; bx <= maxBX; bx++)
            {
                // loop over affected blocks in Y direction
                for (int by = minBY; by <= maxBY; by++)
                {
                    ABlock block;

                    // conversion to dynamic block must be synchronized to prevent 2 independent threads converting same block
                    lock (_blocks)
                    {
                        // Get related block
                        block = _blocks[bx][by];

                        // check for dynamic block
                        if (!(block is IBlockDynamic))
                        {
                            // null block means no geodata (particular region file is not loaded), no geodata means no geobjects
                            if (block is BlockNull)
                                continue;

                            // not a dynamic block, convert it
                            if (block is BlockFlat)
                            {
                                // convert flat block to the dynamic complex block
                                block = new BlockComplexDynamic(bx, by, (BlockFlat)block);
                                _blocks[bx][by] = block;
                            }

                            else if (block is BlockComplex)
                            {
                                // convert complex block to the dynamic complex block
                                block = new BlockComplexDynamic(bx, by, (BlockComplex)block);
                                _blocks[bx][by] = block;
                            }

                            else if (block is BlockMultilayer)
                            {
                                // convert multilayer block to the dynamic multilayer block
                                block = new BlockMultilayerDynamic(bx, by, (BlockMultilayer)block);
                                _blocks[bx][by] = block;
                            }
                        }
                    }

                    // add/remove geo object to/from dynamic block
                    if (add)
                        ((IBlockDynamic)block).addGeoObject(geoObject);
                    else
                        ((IBlockDynamic)block).removeGeoObject(geoObject);
                }
            }
        }

        // PATHFINDING

        /**
         * Check line of sight from {@link WorldObject} to {@link WorldObject}.
         * @param origin : The origin object.
         * @param tarGet : The tarGet object.
         * @return {@code bool} : True if origin can see tarGet
         */
        public bool canSeeTarGet(WorldObject origin, WorldObject tarGet)
        {
            // Get origin and tarGet world coordinates
            int ox = origin.GetX();
            int oy = origin.GetY();
            int oz = origin.GetZ();
            int tx = tarGet.GetX();
            int ty = tarGet.GetY();
            int tz = tarGet.GetZ();

            // Get origin and check existing geo coordinates
            int gox = GetGeoX(ox);
            int goy = GetGeoY(oy);
            if (!hasGeoPos(gox, goy))
                return true;

            short goz = GetHeightNearest(gox, goy, oz);

            // Get tarGet and check existing geo coordinates
            int gtx = GetGeoX(tx);
            int gty = GetGeoY(ty);
            if (!hasGeoPos(gtx, gty))
                return true;

            bool door = tarGet is Door;
            short gtz = door ? GetHeightNearestOriginal(gtx, gty, tz) : GetHeightNearest(gtx, gty, tz);

            // origin and tarGet coordinates are same
            if (gox == gtx && goy == gty)
                return goz == gtz;

            // Get origin and tarGet height, real height = collision height * 2
            double oheight = 0;
            if (origin is Creature)
                oheight = ((Creature)origin).GetCollisionHeight() * 2;

            double theight = 0;
            if (tarGet is Creature)
                theight = ((Creature)tarGet).GetCollisionHeight() * 2;

            // perform geodata check
            return door ? checkSeeOriginal(gox, goy, goz, oheight, gtx, gty, gtz, theight) : checkSee(gox, goy, goz, oheight, gtx, gty, gtz, theight);
        }

        /**
         * Check line of sight from {@link WorldObject} to {@link Location}.
         * @param origin : The origin object.
         * @param position : The tarGet position.
         * @return {@code bool} : True if object can see position
         */
        public bool canSeeTarGet(WorldObject origin, Location position)
        {
            // Get origin and tarGet world coordinates
            int ox = origin.GetX();
            int oy = origin.GetY();
            int oz = origin.GetZ();
            int tx = position.GetX();
            int ty = position.GetY();
            int tz = position.GetZ();

            // Get origin and check existing geo coordinates
            int gox = GetGeoX(ox);
            int goy = GetGeoY(oy);
            if (!hasGeoPos(gox, goy))
                return true;

            short goz = GetHeightNearest(gox, goy, oz);

            // Get tarGet and check existing geo coordinates
            int gtx = GetGeoX(tx);
            int gty = GetGeoY(ty);
            if (!hasGeoPos(gtx, gty))
                return true;

            short gtz = GetHeightNearest(gtx, gty, tz);

            // origin and tarGet coordinates are same
            if (gox == gtx && goy == gty)
                return goz == gtz;

            // Get origin and tarGet height, real height = collision height * 2
            double oheight = 0;
            if (origin is Creature)
                oheight = ((Creature)origin).GetTemplate().GetCollisionHeight();

            // perform geodata check
            return checkSee(gox, goy, goz, oheight, gtx, gty, gtz, 0);
        }

        /**
         * Simple check for origin to tarGet visibility.
         * @param gox : origin X geodata coordinate
         * @param goy : origin Y geodata coordinate
         * @param goz : origin Z geodata coordinate
         * @param oheight : origin height (if instance of {@link Creature})
         * @param gtx : tarGet X geodata coordinate
         * @param gty : tarGet Y geodata coordinate
         * @param gtz : tarGet Z geodata coordinate
         * @param theight : tarGet height (if instance of {@link Creature})
         * @return {@code bool} : True, when tarGet can be seen.
         */
        protected bool checkSee(int gox, int goy, int goz, double oheight, int gtx, int gty, int gtz, double theight)
        {
            // Get line of sight Z coordinates
            double losoz = goz + oheight * geodataConfig.PartOfCharacterHeight / 100;
            double lostz = gtz + theight * geodataConfig.PartOfCharacterHeight / 100;

            // Get X delta and signum
            int dx = Math.Abs(gtx - gox);
            int sx = gox < gtx ? 1 : -1;
            byte dirox = sx > 0 ? GeoStructure.CellFlagE : GeoStructure.CellFlagW;
            byte dirtx = sx > 0 ? GeoStructure.CellFlagW : GeoStructure.CellFlagE;

            // Get Y delta and signum
            int dy = Math.Abs(gty - goy);
            int sy = goy < gty ? 1 : -1;
            byte diroy = sy > 0 ? GeoStructure.CellFlagS : GeoStructure.CellFlagN;
            byte dirty = sy > 0 ? GeoStructure.CellFlagN : GeoStructure.CellFlagS;

            // Get Z delta
            int dm = Math.Max(dx, dy);
            double dz = (lostz - losoz) / dm;

            // Get direction flag for diagonal movement
            byte diroxy = GetDirXY(dirox, diroy);
            byte dirtxy = GetDirXY(dirtx, dirty);

            // delta, determines axis to move on (+..X axis, -..Y axis)
            int d = dx - dy;

            // NSWE direction of movement
            byte diro;
            byte dirt;

            // clearDebugItems();
            // dropDebugItem(728, 0, new GeoLocation(gox, goy, goz)); // blue potion
            // dropDebugItem(728, 0, new GeoLocation(gtx, gty, gtz)); // blue potion

            // initialize node values
            int nox = gox;
            int noy = goy;
            int ntx = gtx;
            int nty = gty;
            byte nsweo = GetNsweNearest(gox, goy, goz);
            byte nswet = GetNsweNearest(gtx, gty, gtz);

            // loop
            ABlock block;
            int index;
            for (int i = 0; i < (dm + 1) / 2; i++)
            {
                // dropDebugItem(57, 0, new GeoLocation(gox, goy, goz)); // antidote
                // dropDebugItem(1831, 0, new GeoLocation(gtx, gty, gtz)); // adena

                // reset direction flag
                diro = 0;
                dirt = 0;

                // calculate next point coordinates
                int e2 = 2 * d;
                if (e2 > -dy && e2 < dx)
                {
                    // calculate next point XY coordinates
                    d -= dy;
                    d += dx;
                    nox += sx;
                    ntx -= sx;
                    noy += sy;
                    nty -= sy;
                    diro |= diroxy;
                    dirt |= dirtxy;
                }
                else if (e2 > -dy)
                {
                    // calculate next point X coordinate
                    d -= dy;
                    nox += sx;
                    ntx -= sx;
                    diro |= dirox;
                    dirt |= dirtx;
                }
                else if (e2 < dx)
                {
                    // calculate next point Y coordinate
                    d += dx;
                    noy += sy;
                    nty -= sy;
                    diro |= diroy;
                    dirt |= dirty;
                }

                {
                    // Get block of the next cell
                    block = GetBlock(nox, noy);

                    // Get index of particular layer, based on movement conditions
                    if ((nsweo & diro) == 0)
                        index = block.GetIndexAbove(nox, noy, goz - GeoStructure.CellIgnoreHeight);
                    else
                        index = block.GetIndexBelow(nox, noy, goz + GeoStructure.CellIgnoreHeight);

                    // layer does not exist, return
                    if (index == -1)
                        return false;

                    // Get layer and next line of sight Z coordinate
                    goz = block.GetHeight(index);
                    losoz += dz;

                    // perform line of sight check, return when fails
                    if ((goz - losoz) > geodataConfig.MaxObstacleHeight)
                        return false;

                    // Get layer nswe
                    nsweo = block.GetNswe(index);
                }
                {
                    // Get block of the next cell
                    block = GetBlock(ntx, nty);

                    // Get index of particular layer, based on movement conditions
                    if ((nswet & dirt) == 0)
                        index = block.GetIndexAbove(ntx, nty, gtz - GeoStructure.CellIgnoreHeight);
                    else
                        index = block.GetIndexBelow(ntx, nty, gtz + GeoStructure.CellIgnoreHeight);

                    // layer does not exist, return
                    if (index == -1)
                        return false;

                    // Get layer and next line of sight Z coordinate
                    gtz = block.GetHeight(index);
                    lostz -= dz;

                    // perform line of sight check, return when fails
                    if ((gtz - lostz) > geodataConfig.MaxObstacleHeight)
                        return false;

                    // Get layer nswe
                    nswet = block.GetNswe(index);
                }

                // update coords
                gox = nox;
                goy = noy;
                gtx = ntx;
                gty = nty;
            }

            // when iteration is completed, compare  Z coordinates
            return Math.Abs(goz - gtz) < GeoStructure.CellHeight * 4;
        }

        /**
         * Simple check for origin to tarGet visibility.<br>
         * Geodata without {@link IGeoObject} are taken in consideration.<br>
         * NOTE: When two doors close between each other and the LoS check of one doors is performed through another door, result will not be accurate (the other door are skipped).
         * @param gox : origin X geodata coordinate
         * @param goy : origin Y geodata coordinate
         * @param goz : origin Z geodata coordinate
         * @param oheight : origin height (if instance of {@link Creature})
         * @param gtx : tarGet X geodata coordinate
         * @param gty : tarGet Y geodata coordinate
         * @param gtz : tarGet Z geodata coordinate
         * @param theight : tarGet height (if instance of {@link Creature} or {@link Door})
         * @return {@code bool} : True, when tarGet can be seen.
         */
        protected bool checkSeeOriginal(int gox, int goy, int goz, double oheight, int gtx, int gty, int gtz, double theight)
        {
            // Get line of sight Z coordinates
            double losoz = goz + oheight * geodataConfig.PartOfCharacterHeight / 100;
            double lostz = gtz + theight * geodataConfig.PartOfCharacterHeight / 100;

            // Get X delta and signum
            int dx = Math.Abs(gtx - gox);
            int sx = gox < gtx ? 1 : -1;
            byte dirox = sx > 0 ? GeoStructure.CellFlagE : GeoStructure.CellFlagW;
            byte dirtx = sx > 0 ? GeoStructure.CellFlagW : GeoStructure.CellFlagE;

            // Get Y delta and signum
            int dy = Math.Abs(gty - goy);
            int sy = goy < gty ? 1 : -1;
            byte diroy = sy > 0 ? GeoStructure.CellFlagS : GeoStructure.CellFlagN;
            byte dirty = sy > 0 ? GeoStructure.CellFlagN : GeoStructure.CellFlagS;

            // Get Z delta
            int dm = Math.Max(dx, dy);
            double dz = (lostz - losoz) / dm;

            // Get direction flag for diagonal movement
            byte diroxy = GetDirXY(dirox, diroy);
            byte dirtxy = GetDirXY(dirtx, dirty);

            // delta, determines axis to move on (+..X axis, -..Y axis)
            int d = dx - dy;

            // NSWE direction of movement
            byte diro;
            byte dirt;

            // clearDebugItems();
            // dropDebugItem(728, 0, new GeoLocation(gox, goy, goz)); // blue potion
            // dropDebugItem(728, 0, new GeoLocation(gtx, gty, gtz)); // blue potion

            // initialize node values
            int nox = gox;
            int noy = goy;
            int ntx = gtx;
            int nty = gty;
            byte nsweo = GetNsweNearestOriginal(gox, goy, goz);
            byte nswet = GetNsweNearestOriginal(gtx, gty, gtz);

            // loop
            ABlock block;
            int index;
            for (int i = 0; i < (dm + 1) / 2; i++)
            {
                // dropDebugItem(57, 0, new GeoLocation(gox, goy, goz)); // antidote
                // dropDebugItem(1831, 0, new GeoLocation(gtx, gty, gtz)); // adena

                // reset direction flag
                diro = 0;
                dirt = 0;

                // calculate next point coordinates
                int e2 = 2 * d;
                if (e2 > -dy && e2 < dx)
                {
                    // calculate next point XY coordinates
                    d -= dy;
                    d += dx;
                    nox += sx;
                    ntx -= sx;
                    noy += sy;
                    nty -= sy;
                    diro |= diroxy;
                    dirt |= dirtxy;
                }
                else if (e2 > -dy)
                {
                    // calculate next point X coordinate
                    d -= dy;
                    nox += sx;
                    ntx -= sx;
                    diro |= dirox;
                    dirt |= dirtx;
                }
                else if (e2 < dx)
                {
                    // calculate next point Y coordinate
                    d += dx;
                    noy += sy;
                    nty -= sy;
                    diro |= diroy;
                    dirt |= dirty;
                }

                {
                    // Get block of the next cell
                    block = GetBlock(nox, noy);

                    // Get index of particular layer, based on movement conditions
                    if ((nsweo & diro) == 0)
                        index = block.GetIndexAboveOriginal(nox, noy, goz - GeoStructure.CellIgnoreHeight);
                    else
                        index = block.GetIndexBelowOriginal(nox, noy, goz + GeoStructure.CellIgnoreHeight);

                    // layer does not exist, return
                    if (index == -1)
                        return false;

                    // Get layer and next line of sight Z coordinate
                    goz = block.GetHeightOriginal(index);
                    losoz += dz;

                    // perform line of sight check, return when fails
                    if ((goz - losoz) > geodataConfig.MaxObstacleHeight)
                        return false;

                    // Get layer nswe
                    nsweo = block.GetNsweOriginal(index);
                }
                {
                    // Get block of the next cell
                    block = GetBlock(ntx, nty);

                    // Get index of particular layer, based on movement conditions
                    if ((nswet & dirt) == 0)
                        index = block.GetIndexAboveOriginal(ntx, nty, gtz - GeoStructure.CellIgnoreHeight);
                    else
                        index = block.GetIndexBelowOriginal(ntx, nty, gtz + GeoStructure.CellIgnoreHeight);

                    // layer does not exist, return
                    if (index == -1)
                        return false;

                    // Get layer and next line of sight Z coordinate
                    gtz = block.GetHeightOriginal(index);
                    lostz -= dz;

                    // perform line of sight check, return when fails
                    if ((gtz - lostz) > geodataConfig.MaxObstacleHeight)
                        return false;

                    // Get layer nswe
                    nswet = block.GetNsweOriginal(index);
                }

                // update coords
                gox = nox;
                goy = noy;
                gtx = ntx;
                gty = nty;
            }

            // when iteration is completed, compare  Z coordinates
            return Math.Abs(goz - gtz) < GeoStructure.CellHeight * 4;
        }

        /**
         * Check movement from coordinates to coordinates.
         * @param ox : origin X coordinate
         * @param oy : origin Y coordinate
         * @param oz : origin Z coordinate
         * @param tx : tarGet X coordinate
         * @param ty : tarGet Y coordinate
         * @param tz : tarGet Z coordinate
         * @return {code bool} : True if tarGet coordinates are reachable from origin coordinates
         */
        public bool canMoveToTarGet(int ox, int oy, int oz, int tx, int ty, int tz)
        {
            // Get origin and check existing geo coordinates
            int gox = GetGeoX(ox);
            int goy = GetGeoY(oy);
            if (!hasGeoPos(gox, goy))
                return true;

            short goz = GetHeightNearest(gox, goy, oz);

            // Get tarGet and check existing geo coordinates
            int gtx = GetGeoX(tx);
            int gty = GetGeoY(ty);
            if (!hasGeoPos(gtx, gty))
                return true;

            short gtz = GetHeightNearest(gtx, gty, tz);

            // tarGet coordinates reached
            if (gox == gtx && goy == gty && goz == gtz)
                return true;

            // perform geodata check
            GeoLocation loc = checkMove(gox, goy, goz, gtx, gty, gtz);
            return loc.GetGeoX() == gtx && loc.GetGeoY() == gty;
        }

        /**
         * Check movement from origin to tarGet. Returns last available point in the checked path.
         * @param ox : origin X coordinate
         * @param oy : origin Y coordinate
         * @param oz : origin Z coordinate
         * @param tx : tarGet X coordinate
         * @param ty : tarGet Y coordinate
         * @param tz : tarGet Z coordinate
         * @return {@link Location} : Last point where object can walk (just before wall)
         */
        public Location canMoveToTarGetLoc(int ox, int oy, int oz, int tx, int ty, int tz)
        {
            // Get origin and check existing geo coordinates
            int gox = GetGeoX(ox);
            int goy = GetGeoY(oy);
            if (!hasGeoPos(gox, goy))
                return new Location(tx, ty, tz);

            short goz = GetHeightNearest(gox, goy, oz);

            // Get tarGet and check existing geo coordinates
            int gtx = GetGeoX(tx);
            int gty = GetGeoY(ty);
            if (!hasGeoPos(gtx, gty))
                return new Location(tx, ty, tz);

            short gtz = GetHeightNearest(gtx, gty, tz);

            // tarGet coordinates reached
            if (gox == gtx && goy == gty && goz == gtz)
                return new Location(tx, ty, tz);

            // perform geodata check
            return checkMove(gox, goy, goz, gtx, gty, gtz);
        }

        /**
         * With this method you can check if a position is visible or can be reached by beeline movement.<br>
         * TarGet X and Y reachable and Z is on same floor:
         * <ul>
         * <li>Location of the tarGet with corrected Z value from geodata.</li>
         * </ul>
         * TarGet X and Y reachable but Z is on another floor:
         * <ul>
         * <li>Location of the origin with corrected Z value from geodata.</li>
         * </ul>
         * TarGet X and Y not reachable:
         * <ul>
         * <li>Last accessible location in destination to tarGet.</li>
         * </ul>
         * @param gox : origin X geodata coordinate
         * @param goy : origin Y geodata coordinate
         * @param goz : origin Z geodata coordinate
         * @param gtx : tarGet X geodata coordinate
         * @param gty : tarGet Y geodata coordinate
         * @param gtz : tarGet Z geodata coordinate
         * @return {@link GeoLocation} : The last allowed point of movement.
         */
        protected GeoLocation checkMove(int gox, int goy, int goz, int gtx, int gty, int gtz)
        {
            // Get X delta, signum and direction flag
            int dx = Math.Abs(gtx - gox);
            int sx = gox < gtx ? 1 : -1;
            byte dirX = sx > 0 ? GeoStructure.CellFlagE : GeoStructure.CellFlagW;

            // Get Y delta, signum and direction flag
            int dy = Math.Abs(gty - goy);
            int sy = goy < gty ? 1 : -1;
            byte dirY = sy > 0 ? GeoStructure.CellFlagS : GeoStructure.CellFlagN;

            // Get direction flag for diagonal movement
            byte dirXY = GetDirXY(dirX, dirY);

            // delta, determines axis to move on (+..X axis, -..Y axis)
            int d = dx - dy;

            // NSWE direction of movement
            byte direction;

            // load pointer coordinates
            int gpx = gox;
            int gpy = goy;
            int gpz = goz;

            // load next pointer
            int nx = gpx;
            int ny = gpy;

            // loop
            do
            {
                direction = 0;

                // calculate next point coordinates
                int e2 = 2 * d;
                if (e2 > -dy && e2 < dx)
                {
                    d -= dy;
                    d += dx;
                    nx += sx;
                    ny += sy;
                    direction |= dirXY;
                }
                else if (e2 > -dy)
                {
                    d -= dy;
                    nx += sx;
                    direction |= dirX;
                }
                else if (e2 < dx)
                {
                    d += dx;
                    ny += sy;
                    direction |= dirY;
                }

                // obstacle found, return
                if ((GetNsweNearest(gpx, gpy, gpz) & direction) == 0)
                    return new GeoLocation(gpx, gpy, gpz);

                // update pointer coordinates
                gpx = nx;
                gpy = ny;
                gpz = GetHeightNearest(nx, ny, gpz);

                // tarGet coordinates reached
                if (gpx == gtx && gpy == gty)
                {
                    if (gpz == gtz)
                    {
                        // path found, Z coordinates are okay, return tarGet point
                        return new GeoLocation(gtx, gty, gtz);
                    }

                    // path found, Z coordinates are not okay, return origin point
                    return new GeoLocation(gox, goy, goz);
                }
            }
            while (true);
        }

        /**
         * Returns diagonal NSWE flag format of combined two NSWE flags.
         * @param dirX : X direction NSWE flag
         * @param dirY : Y direction NSWE flag
         * @return byte : NSWE flag of combined direction
         */
        private static byte GetDirXY(byte dirX, byte dirY)
        {
            // check axis directions
            if (dirY == GeoStructure.CellFlagN)
            {
                if (dirX == GeoStructure.CellFlagW)
                    return GeoStructure.CellFlagNAndW;

                return GeoStructure.CellFlagNAndE;
            }

            if (dirX == GeoStructure.CellFlagW)
                return GeoStructure.CellFlagSAndW;

            return GeoStructure.CellFlagSAndE;
        }

        /**
         * Returns the list of location objects as a result of complete path calculation.
         * @param ox : origin x
         * @param oy : origin y
         * @param oz : origin z
         * @param tx : tarGet x
         * @param ty : tarGet y
         * @param tz : tarGet z
         * @param playable : moving object is playable?
         * @return {@code List<Location>} : complete path from nodes
         */
        public List<Location> findPath(int ox, int oy, int oz, int tx, int ty, int tz, bool playable)
        {
            // Get origin and check existing geo coords
            int gox = GetGeoX(ox);
            int goy = GetGeoY(oy);
            if (!hasGeoPos(gox, goy))
                return null;

            short goz = GetHeightNearest(gox, goy, oz);

            // Get tarGet and check existing geo coords
            int gtx = GetGeoX(tx);
            int gty = GetGeoY(ty);
            if (!hasGeoPos(gtx, gty))
                return null;

            short gtz = GetHeightNearest(gtx, gty, tz);

            // Prepare buffer for pathfinding calculations
            NodeBuffer buffer = GetBuffer(64 + (2 * Math.Max(Math.Abs(gox - gtx), Math.Abs(goy - gty))), playable);
            if (buffer == null)
                return null;

            // clean debug path
            bool debug = playable && geodataConfig.DebugPath;
            if (debug)
                clearDebugItems();

            // find path
            List<Location> path = null;
            try
            {
                Node result = buffer.findPath(gox, goy, goz, gtx, gty, gtz);

                if (result == null)
                {
                    _findFails++;
                    return null;
                }

                if (debug)
                {
                    // path origin
                    dropDebugItem(728, 0, new GeoLocation(gox, goy, goz)); // blue potion

                    // path
                    for (Node n : buffer.debugPath())
                    {
                        if (n.GetCost() < 0)
                            dropDebugItem(1831, (int)(-n.GetCost() * 10), n.GetLoc()); // antidote
                        else
                            dropDebugItem(57, (int)(n.GetCost() * 10), n.GetLoc()); // adena
                    }
                }

                path = ConstructPath(result);
            }

            catch (Exception e)
            {
                logger.Error("Failed to generate a path.", e);

                _findFails++;
                return null;
            }
            finally
            {
                buffer.free();
                _findSuccess++;
            }

            // check path
            if (path.Count < 3)
                return path;

            // log data
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _postFilterUses++;
            if (playable)
                _postFilterPlayableUses++;

            // Get path list iterator
            ListIterator<Location> point = path.listIterator();

            // Get node A (origin)
            int nodeAx = gox;
            int nodeAy = goy;
            short nodeAz = goz;

            // Get node B
            GeoLocation nodeB = (GeoLocation)point.next();

            // iterate thought the path to optimize it
            while (point.hasNext())
            {
                // Get node C
                GeoLocation nodeC = (GeoLocation)path.Get(point.nextIndex());

                // check movement from node A to node C
                GeoLocation loc = checkMove(nodeAx, nodeAy, nodeAz, nodeC.GetGeoX(), nodeC.GetGeoY(), nodeC.GetZ());
                if (loc.GetGeoX() == nodeC.GetGeoX() && loc.GetGeoY() == nodeC.GetGeoY())
                {
                    // can move from node A to node C

                    // remove node B
                    point.remove();

                    // show skipped nodes
                    if (debug)
                        dropDebugItem(735, 0, nodeB); // green potion
                }
                else
                {
                    // can not move from node A to node C

                    // set node A (node B is part of path, update A coordinates)
                    nodeAx = nodeB.GetGeoX();
                    nodeAy = nodeB.GetGeoY();
                    nodeAz = (short)nodeB.GetZ();
                }

                // set node B
                nodeB = (GeoLocation)point.next();
            }

            // show  path
            if (debug)
            {
                foreach (Location node in path)
                    dropDebugItem(65, 0, node); // red potion
            }

            // log data
            stopWatch.Stop();
            _postFilterElapsed += stopWatch.ElapsedMilliseconds;

            return path;
        }

        /**
         * Return pathfinding statistics, useful for Getting information about pathfinding status.
         * @return {@code List<string>} : stats
         */
        public List<string> GetStat()
        {
            List<string> list = new List<string>();

            foreach (BufferHolder buffer in _buffers)
                list.Add(buffer.ToString());

            list.Add("Use: playable=" + string.valueOf(_postFilterPlayableUses) + " non-playable=" + string.valueOf(_postFilterUses - _postFilterPlayableUses));

            if (_postFilterUses > 0)
                list.Add("Time (ms): total=" + string.valueOf(_postFilterElapsed) + " avg=" + string.format("%1.2f", (double)_postFilterElapsed / _postFilterUses));

            list.Add("Pathfind: success=" + string.valueOf(_findSuccess) + ", fail=" + string.valueOf(_findFails));

            return list;
        }

        // MISC

        /**
         * Record a geodata bug.
         * @param loc : Location of the geodata bug.
         * @param comment : Short commentary.
         * @return bool : True, when bug was successfully recorded.
         */
        public bool addGeoBug(Location loc, string comment)
        {
            int gox = GetGeoX(loc.GetX());
            int goy = GetGeoY(loc.GetY());
            int goz = loc.GetZ();
            int rx = gox / GeoStructure.RegionCellsX + GeoStructure.TileXMin;
            int ry = goy / GeoStructure.RegionCellsY + GeoStructure.TileYMin;
            int bx = (gox / GeoStructure.BlockCellsX) % GeoStructure.RegionBlocksX;
            int by = (goy / GeoStructure.BlockCellsY) % GeoStructure.RegionBlocksY;
            int cx = gox % GeoStructure.BlockCellsX;
            int cy = goy % GeoStructure.BlockCellsY;

            try
            {
                _geoBugReports.printf(GEO_BUG, rx, ry, bx, by, cx, cy, goz, comment.replace(";", ":"));
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Couldn't save new entry to \"geo_bugs.txt\" file.", e);
                return false;
            }
        }

        /**
         * Add new item to drop list for debug purpose.
         * @param id : Item id
         * @param count : Item count
         * @param loc : Item location
         */
        public void dropDebugItem(int id, int count, Location loc)
        {
            ItemInstance item = new ItemInstance(IdFactory.GetInstance().GetNextId(), id);
            item.setCount(count);
            item.spawnMe(loc.GetX(), loc.GetY(), loc.GetZ());
            _debugItems.add(item);
        }

        /**
         * Clear item drop list for debugging paths.
         */
        public void clearDebugItems()
        {
            foreach (ItemInstance item in _debugItems)
                item.decayMe();

            _debugItems.clear();
        }




        /**
         * Returns the instance of the {@link GeoEngine}.
         * @return {@link GeoEngine} : The instance.
         */
        public static GeoEngine GetInstance()
        {
            return SingletonHolder.INSTANCE;
        }

        private static class SingletonHolder
        {
            protected static GeoEngine INSTANCE = new GeoEngine();
        }
    }
}
