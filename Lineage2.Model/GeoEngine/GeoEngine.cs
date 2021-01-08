using Lineage2.Model.Configs;
using Lineage2.Model.GeoEngine.GeoData;
using Lineage2.Model.Locations;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Lineage2.Model.GeoEngine
{
    public class GeoEngine
    {
        private readonly ILogger logger = Log.Logger.ForContext<GeoEngine>();
        private GeodataConfig geodataConfig;
        private static byte _e = 1;
        private static byte _w = 2;
        private static byte _s = 4;
        private static byte _n = 8;
        private static Dictionary<short, MemoryMappedViewAccessor> _geodata = new Dictionary<short, MemoryMappedViewAccessor>();
        private static Dictionary<short, int[]> _geodataIndex = new Dictionary<short, int[]>();
        //private static Dictionary<short, FileChannel> Geodata_files = new Dictionary<short, FileChannel>();

        private List<GeoDateFileInfo> GeoDateFileInfos;

        public GeoEngine()//GeodataConfig geodataConfig)
        {
            GeoDateFileInfos = new List<GeoDateFileInfo>() {
                //new GeoDateFileInfo(){PositionX = 11, PositionY=23 },
                //new GeoDateFileInfo(){PositionX = 11, PositionY=24 },
                //new GeoDateFileInfo(){PositionX = 11, PositionY=25 },
                //new GeoDateFileInfo(){PositionX = 11, PositionY=26 },
                //new GeoDateFileInfo(){PositionX = 12, PositionY=23 },
                //new GeoDateFileInfo(){PositionX = 12, PositionY=24 },
                //new GeoDateFileInfo(){PositionX = 12, PositionY=25 },
                //new GeoDateFileInfo(){PositionX = 12, PositionY=26 },
                //new GeoDateFileInfo(){PositionX = 13, PositionY=23 },
                //new GeoDateFileInfo(){PositionX = 13, PositionY=24 },
                //new GeoDateFileInfo(){PositionX = 13, PositionY=25 },
                //new GeoDateFileInfo(){PositionX = 13, PositionY=26 },
                //new GeoDateFileInfo(){PositionX = 14, PositionY=23 },
                //new GeoDateFileInfo(){PositionX = 14, PositionY=24 },
                //new GeoDateFileInfo(){PositionX = 14, PositionY=25 },
                //new GeoDateFileInfo(){PositionX = 14, PositionY=26 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=18 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=19 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=20 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=21 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=22 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=23 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=24 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=25 },
                //new GeoDateFileInfo(){PositionX = 15, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 16, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 17, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 18, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 19, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 20, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 21, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 22, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 23, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=22 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=23 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=24 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=25 },
                new GeoDateFileInfo(){PositionX = 24, PositionY=26 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=10 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=12 },
                //new GeoDateFileInfo(){PositionX = 25, PositionY=13 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=16 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=17 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=18 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=19 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=20 },
                new GeoDateFileInfo(){PositionX = 25, PositionY=21 },
                new GeoDateFileInfo(){PositionX = 26, PositionY=11 },
                new GeoDateFileInfo(){PositionX = 26, PositionY=12 },
                new GeoDateFileInfo(){PositionX = 26, PositionY=14 },
                new GeoDateFileInfo(){PositionX = 26, PositionY=15 },
                new GeoDateFileInfo(){PositionX = 26, PositionY=16 }
            };
            //this.geodataConfig = geodataConfig;
            nInitGeodata();
        }

        public Location moveCheck(int x, int y, int z, int tx, int ty, int tz)
        {
            Location startpoint = new Location(x, y, z);

            //if (DoorTable.getInstance().checkIfDoorsBetween(x, y, z, tx, ty, tz))
            //    return startpoint;

            Location destiny = new Location(tx, ty, tz);
            return moveCheck(startpoint, destiny, (x - GeoStructure.WorldXMin) >> 4, (y - GeoStructure.WorldYMin) >> 4, z, (tx - GeoStructure.WorldXMin) >> 4, (ty - GeoStructure.WorldYMin) >> 4, tz);
        }

        private Location moveCheck(Location startpoint, Location destiny, int x, int y, double z, int tx, int ty, int tz)
        {
            int dx = (tx - x);
            int dy = (ty - y);
            int distance2 = dx * dx + dy * dy;

            if (distance2 == 0)
                return destiny;
            if (distance2 > 36100) // 190*190*16 = 3040 world coord
            {
                // Avoid too long check
                // Currently we calculate a middle point
                // for wyvern users and otherwise for comfort
                double divider = Math.Sqrt((double)30000 / distance2);
                tx = x + (int)(divider * dx);
                ty = y + (int)(divider * dy);
                int dz = (tz - startpoint.getZ());
                tz = startpoint.getZ() + (int)(divider * dz);
                dx = (tx - x);
                dy = (ty - y);
                //return startpoint;
            }

            // Increment in Z coordinate when moving along X or Y axis 
            // and not straight to the target. This is done because
            // calculation moves either in X or Y direction.
            int inc_x = sign(dx);
            int inc_y = sign(dy);
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            // next_* are used in NcanMoveNext check from x,y
            int next_x = x;
            int next_y = y;
            double tempz = z;

            // creates path to the target, using only x or y direction
            // calculation stops when next_* == target
            if (dx >= dy)// dy/dx <= 1
            {
                int delta_A = 2 * dy;
                int d = delta_A - dx;
                int delta_B = delta_A - 2 * dx;

                for (int i = 0; i < dx; i++)
                {
                    x = next_x;
                    y = next_y;
                    if (d > 0)
                    {
                        d += delta_B;
                        next_x += inc_x;
                        next_y += inc_y;
                        //_log.warn("2: next_x:"+next_x+" next_y"+next_y);
                        tempz = nCanMoveNext(x, y, (int)z, next_x, next_y, tz);
                        if (tempz == double.MinValue)
                            return new Location((x << 4) + GeoStructure.WorldXMin, (y << 4) + GeoStructure.WorldYMin, (int)z);
                        z = tempz;
                    }
                    else
                    {
                        d += delta_A;
                        next_x += inc_x;
                        //_log.warn("3: next_x:"+next_x+" next_y"+next_y);
                        tempz = nCanMoveNext(x, y, (int)z, next_x, next_y, tz);
                        if (tempz == double.MinValue)
                            return new Location((x << 4) + GeoStructure.WorldXMin, (y << 4) + GeoStructure.WorldYMin, (int)z);
                        z = tempz;
                    }
                }
            }
            else
            {
                int delta_A = 2 * dx;
                int d = delta_A - dy;
                int delta_B = delta_A - 2 * dy;
                for (int i = 0; i < dy; i++)
                {
                    x = next_x;
                    y = next_y;
                    if (d > 0)
                    {
                        d += delta_B;
                        next_y += inc_y;
                        next_x += inc_x;
                        //_log.warn("5: next_x:"+next_x+" next_y"+next_y);
                        tempz = nCanMoveNext(x, y, (int)z, next_x, next_y, tz);
                        if (tempz == double.MinValue)
                            return new Location((x << 4) + GeoStructure.WorldXMin, (y << 4) + GeoStructure.WorldYMin, (int)z);
                        z = tempz;
                    }
                    else
                    {
                        d += delta_A;
                        next_y += inc_y;
                        //_log.warn("6: next_x:"+next_x+" next_y"+next_y);
                        tempz = nCanMoveNext(x, y, (int)z, next_x, next_y, tz);
                        if (tempz == double.MinValue)
                            return new Location((x << 4) + GeoStructure.WorldXMin, (y << 4) + GeoStructure.WorldYMin, (int)z);
                        z = tempz;
                    }
                }
            }
            return destiny; // should actually return correct z here instead of tz
        }

        /**
        * @param x
        * @param y
        * @param z
        * @param tx
        * @param ty
        * @param tz
        * @return True if char can move to (tx,ty,tz)
        */
        private double nCanMoveNext(int x, int y, int z, int tx, int ty, int tz)
        {
            short region = getRegionOffset(x, y);
            int blockX = getBlock(x);
            int blockY = getBlock(y);
            int cellX, cellY;
            short NSWE = 0;

            int index = 0;

            if (_geodataIndex.TryGetValue(region, out int[] data))
            {
                //Get Index for current block of current region geodata
                index = data[(blockX << 8) + (blockY)];
            }
            else
            {
                //Geodata without index - it is just empty so index can be calculated on the fly
                index = ((blockX << 8) + blockY) * 3;
            }

            //Buffer that Contains current Region GeoData
            MemoryMappedViewAccessor geo = _geodata[region];

            if (geo == null)
            {
                logger.Warning("Geo Region - Region Offset: " + region + " dosnt exist!!");
                return z;
            }
            //Read current block type: 0-flat,1-complex,2-multilevel
            byte type = geo.ReadByte(index);
            index++;
            if (type == 0) //flat
                return z;
            else if (type == 1) //complex
            {
                cellX = getCell(x);
                cellY = getCell(y);
                index += ((cellX << 3) + cellY) << 1;
                short height = geo.ReadInt16(index);
                NSWE = (short)(height & 0x0F);
                height = (short)(height & 0x0fff0);
                height = (short)(height >> 1); //height / 2
                if (checkNSWE(NSWE, x, y, tx, ty))
                    return height;
                return double.MinValue;
            }
            else //multilevel, type == 2
            {
                cellX = getCell(x);
                cellY = getCell(y);
                int offset = (cellX << 3) + cellY;
                while (offset > 0) // iterates (too many times?) to get to layer count
                {
                    byte lc = geo.ReadByte(index);
                    index += (lc << 1) + 1;
                    offset--;
                }
                byte layers = geo.ReadByte(index);
                //_log.warn("layers"+layers);
                index++;
                short height = -1;
                if (layers <= 0 || layers > 125)
                {
                    logger.Warning("Broken geofile (case3), region: " + region + " - invalid layer count: " + layers + " at: " + x + " " + y);
                    return z;
                }
                short tempz = short.MinValue;
                while (layers > 0)
                {
                    height = geo.ReadInt16(index);
                    height = (short)(height & 0x0fff0);
                    height = (short)(height >> 1); //height / 2

                    // searches the closest layer to current z coordinate
                    if ((z - tempz) * (z - tempz) > (z - height) * (z - height))
                    {
                        //layercurr = layers;
                        tempz = height;
                        NSWE = geo.ReadInt16(index);
                        NSWE = (short)(NSWE & 0x0F);
                    }
                    layers--;
                    index += 2;
                }
                if (checkNSWE(NSWE, x, y, tx, ty))
                    return tempz;
                return double.MinValue;
            }
        }

        //public short getType(int x, int y)
        //{
        //    return nGetType((x - GeoStructure.WorldXMin) >> 4, (y - GeoStructure.WorldYMin) >> 4);
        //}

        //public short getHeight(int x, int y, int z)
        //{
        //    return nGetHeight((x - GeoStructure.WorldXMin) >> 4, (y - GeoStructure.WorldYMin) >> 4, z);
        //}

        //public short getSpawnHeight(int x, int y, int zmin, int zmax, int spawnid)
        //{
        //    return nGetSpawnHeight((x - GeoStructure.WorldXMin) >> 4, (y - GeoStructure.WorldYMin) >> 4, zmin, zmax, spawnid);
        //}

        public string geoPosition(int x, int y)
        {
            int gx = (x - GeoStructure.WorldXMin) >> 4;
            int gy = (y - GeoStructure.WorldYMin) >> 4;
            return "bx: " + getBlock(gx) + " by: " + getBlock(gy) + " cx: " + getCell(gx) + " cy: " + getCell(gy) + "  region offset: " + getRegionOffset(gx, gy);
        }

        //public bool canSeeTarget(L2Object cha, L2Object target)
        //{
        //    // To be able to see over fences and give the player the viewpoint
        //    // game client has, all coordinates are lifted 45 from ground.
        //    // Because of layer selection in LOS algorithm (it selects -45 there
        //    // and some layers can be very close...) do not change this without 
        //    // changing the LOS code.
        //    // Basically the +45 is character height. Raid bosses are naturally higher,
        //    // dwarves shorter, but this should work relatively well.
        //    // If this is going to be improved, use e.g. 
        //    // ((L2Character)cha).getTemplate().collisionHeight

        //    int z = (int)cha.Position.z + 45;

        //    //TODO: СanSeeTarget
        //    //if (cha is L2SiegeGuardInstance) 
        //    //    z += 30; // well they don't move closer to balcony fence at the moment :(

        //    int z2 = (int)target.Position.z + 45;

        //    //TODO: СanSeeTarget
        //    //if (!(target is L2DoorInstance)
        //    //    && DoorTable.getInstance().checkIfDoorsBetween(cha.Position.x, cha.Position.y, z, target.getX(), target.getY(), z2))
        //    //    return false;

        //    //if (target instanceof L2DoorInstance)
        //    //        return true; // door coordinates are hinge coords..

        //    //if (target instanceof L2SiegeGuardInstance)
        //    //        z2 += 30; // well they don't move closer to balcony fence at the moment :(

        //    if (cha.Position.z >= target.Position.z)
        //        return canSeeTarget(cha.getX(), cha.getY(), z, target.getX(), target.getY(), z2);
        //    else
        //        return canSeeTarget(target.getX(), target.getY(), z2, cha.getX(), cha.getY(), z);
        //}

        /**
		 * @see net.sf.l2j.gameserver.GeoData#canSeeTargetDebug(net.sf.l2j.gameserver.model.actor.instance.L2PcInstance, net.sf.l2j.gameserver.model.L2Object)
		 */
        //public bool canSeeTargetDebug(L2PcInstance gm, L2Object target)
        //{
        //    //// comments: see above
        //    //int z = gm.getZ() + 45;
        //    //int z2 = target.getZ() + 45;
        //    //if (target is L2DoorInstance)
        //    //{
        //    //    gm.sendMessage("door always true");
        //    //    return true; // door coordinates are hinge coords..
        //    //}

        //    //if (gm.getZ() >= target.getZ())
        //    //    return canSeeDebug(gm, (gm.getX() - GeoStructure.WorldXMin) >> 4, (gm.getY() - GeoStructure.WorldYMin) >> 4, z, (target.getX() - GeoStructure.WorldXMin) >> 4, (target.getY() - GeoStructure.WorldYMin) >> 4, z2);
        //    //else
        //    //    return canSeeDebug(gm, (target.getX() - GeoStructure.WorldXMin) >> 4, (target.getY() - GeoStructure.WorldYMin) >> 4, z2, (gm.getX() - GeoStructure.WorldXMin) >> 4, (gm.getY() - GeoStructure.WorldYMin) >> 4, z);
        //}

        /**
		 * @see net.sf.l2j.gameserver.GeoData#getNSWE(int, int, int)
		 */
        //public short getNSWE(int x, int y, int z)
        //{
        //    return nGetNSWE((x - GeoStructure.WorldXMin) >> 4, (y - GeoStructure.WorldYMin) >> 4, z);
        //}

        /**
		 * @see net.sf.l2j.gameserver.GeoData#addGeoDataBug(net.sf.l2j.gameserver.model.actor.instance.L2PcInstance, java.lang.string)
		 */
        //public void addGeoDataBug(L2PcInstance gm, string comment)
        //{
        //    int gx = (gm.getX() - GeoStructure.WorldXMin) >> 4;
        //    int gy = (gm.getY() - GeoStructure.WorldYMin) >> 4;
        //    int bx = getBlock(gx);
        //    int by = getBlock(gy);
        //    int cx = getCell(gx);
        //    int cy = getCell(gy);
        //    int rx = (gx >> 11) + 16;
        //    int ry = (gy >> 11) + 10;
        //    string outStr = rx + ";" + ry + ";" + bx + ";" + by + ";" + cx + ";" + cy + ";" + gm.getZ() + ";" + comment + "\n";
        //    try
        //    {
        //        //TODO: Тут пытался сохранить окшибку и отправлял её гм'у. Надо вставить сюда логгирование и нормально.
        //    }
        //    catch (Exception e)
        //    {
        //        //TODO: Тут отправлял гм'у сообщение. Надо вставить сюда логгирование и нормально.
        //    }
        //}


        //public bool canSeeTarget(Vector3 position, Vector3 targetPosition)
        //{
        //    return canSee((position.x - GeoStructure.WorldXMin) >> 4, (position.y - GeoStructure.WorldYMin) >> 4, position.z, (targetPosition.x - GeoStructure.WorldXMin) >> 4, (targetPosition.y - GeoStructure.WorldYMin) >> 4, targetPosition.z);
        //}

        //private static bool canSee(int x, int y, double z, int tx, int ty, int tz)
        //{
        //    int dx = (tx - x);
        //    int dy = (ty - y);
        //    double dz = (tz - z);
        //    int distance2 = dx * dx + dy * dy;

        //    if (distance2 > 90000) // (300*300) 300*16 = 4800 in world coord
        //    {
        //        //Avoid too long check
        //        return false;
        //    }
        //    // very short checks: 9 => 144 world distance
        //    // this ensures NLOS function has enough points to calculate,
        //    // it might not work when distance is small and path vertical
        //    else if (distance2 < 82)
        //    {
        //        // 200 too deep/high. This value should be in sync with NLOS
        //        if (dz * dz > 40000)
        //        {
        //            short region = getRegionOffset(x, y);
        //            // geodata is loaded for region and mobs should have correct Z coordinate...
        //            // so there would likely be a floor in between the two
        //            if (_geodata.ContainsKey(region))
        //                return false;
        //        }
        //        return true;
        //    }

        // Increment in Z coordinate when moving along X or Y axis 
        // and not straight to the target. This is done because
        // calculation moves either in X or Y direction.
        //        int inc_x = sign(dx);
        //        int inc_y = sign(dy);
        //        dx = Math.Abs(dx);
        //            dy = Math.Abs(dy);
        //            double inc_z_directionx = dz * dx / (distance2);
        //        double inc_z_directiony = dz * dy / (distance2);

        //        // next_* are used in NLOS check from x,y
        //        int next_x = x;
        //        int next_y = y;

        //            // creates path to the target
        //            // calculation stops when next_* == target
        //            if (dx >= dy)// dy/dx <= 1
        //            {
        //                int delta_A = 2 * dy;
        //        int d = delta_A - dx;
        //        int delta_B = delta_A - 2 * dx;

        //                for (int i = 0; i<dx; i++)
        //                {
        //                    x = next_x;
        //                    y = next_y;
        //                    if (d > 0)
        //                    {
        //                        d += delta_B;
        //                        next_x += inc_x;
        //                        z += inc_z_directionx;
        //                        next_y += inc_y;
        //                        z += inc_z_directiony;
        //                        //_log.warn("1: next_x:"+next_x+" next_y"+next_y);
        //                        if (!nLOS(x, y, (int) z, inc_x, inc_y, tz, false))
        //                            return false;
        //                    }
        //                    else
        //                    {
        //                        d += delta_A;
        //                        next_x += inc_x;
        //                        //_log.warn("2: next_x:"+next_x+" next_y"+next_y);
        //                        z += inc_z_directionx;
        //                        if (!nLOS(x, y, (int) z, inc_x, 0, tz, false))
        //                            return false;
        //                    }
        //                }
        //            }
        //            else
        //{
        //    int delta_A = 2 * dx;
        //    int d = delta_A - dy;
        //    int delta_B = delta_A - 2 * dy;
        //    for (int i = 0; i < dy; i++)
        //    {
        //        x = next_x;
        //        y = next_y;
        //        if (d > 0)
        //        {
        //            d += delta_B;
        //            next_y += inc_y;
        //            z += inc_z_directiony;
        //            next_x += inc_x;
        //            z += inc_z_directionx;
        //            //_log.warn("3: next_x:"+next_x+" next_y"+next_y);
        //            if (!nLOS(x, y, (int)z, inc_x, inc_y, tz, false))
        //                return false;
        //        }
        //        else
        //        {
        //            d += delta_A;
        //            next_y += inc_y;
        //            //_log.warn("4: next_x:"+next_x+" next_y"+next_y);
        //            z += inc_z_directiony;
        //            if (!nLOS(x, y, (int)z, 0, inc_y, tz, false))
        //                return false;
        //        }
        //    }
        //}
        //return true;
        //        }

        /*
		 * Debug function for checking if there's a line of sight between
		 * two coordinates.
		 * 
		 * Creates points for line of sight check (x,y,z towards target) and 
		 * in each point, layer and movement checks are made with NLOS function.
		 * 
		 * Coordinates here are geodata x,y but z coordinate is world coordinate
		 */
        //private static bool canSeeDebug(L2PcInstance gm, int x, int y, double z, int tx, int ty, int tz)
        //{
        //    int dx = (tx - x);
        //    int dy = (ty - y);
        //    double dz = (tz - z);
        //    int distance2 = dx * dx + dy * dy;

        //    if (distance2 > 90000) // (300*300) 300*16 = 4800 in world coord
        //    {
        //        //Avoid too long check
        //        gm.sendMessage("dist > 300");
        //        return false;
        //    }
        //    // very short checks: 9 => 144 world distance
        //    // this ensures NLOS function has enough points to calculate,
        //    // it might not work when distance is small and path vertical
        //    else if (distance2 < 82)
        //    {
        //        // 200 too deep/high. This value should be in sync with NLOS
        //        if (dz * dz > 40000)
        //        {
        //            short region = getRegionOffset(x, y);
        //            // geodata is loaded for region and mobs should have correct Z coordinate...
        //            // so there would likely be a floor in between the two
        //            if (_geodata.ContainsKey(region))
        //                return false;
        //        }
        //        return true;
        //    }

        //    // Increment in Z coordinate when moving along X or Y axis 
        //    // and not straight to the target. This is done because
        //    // calculation moves either in X or Y direction.
        //    int inc_x = sign(dx);
        //    int inc_y = sign(dy);
        //    dx = Math.Abs(dx);
        //    dy = Math.Abs(dy);
        //    double inc_z_directionx = dz * dx / (distance2);
        //    double inc_z_directiony = dz * dy / (distance2);

        //    gm.sendMessage("Los: from X: " + x + "Y: " + y + "--->> X: " + tx + " Y: " + ty);

        //    // next_* are used in NLOS check from x,y
        //    int next_x = x;
        //    int next_y = y;

        //    // creates path to the target
        //    // calculation stops when next_* == target
        //    if (dx >= dy)// dy/dx <= 1
        //    {
        //        int delta_A = 2 * dy;
        //        int d = delta_A - dx;
        //        int delta_B = delta_A - 2 * dx;

        //        for (int i = 0; i < dx; i++)
        //        {
        //            x = next_x;
        //            y = next_y;
        //            if (d > 0)
        //            {
        //                d += delta_B;
        //                next_x += inc_x;
        //                z += inc_z_directionx;
        //                next_y += inc_y;
        //                z += inc_z_directiony;
        //                //_log.warn("1: next_x:"+next_x+" next_y"+next_y);
        //                if (!nLOS(x, y, (int)z, inc_x, inc_y, tz, true))
        //                    return false;
        //            }
        //            else
        //            {
        //                d += delta_A;
        //                next_x += inc_x;
        //                //_log.warn("2: next_x:"+next_x+" next_y"+next_y);
        //                z += inc_z_directionx;
        //                if (!nLOS(x, y, (int)z, inc_x, 0, tz, true))
        //                    return false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        int delta_A = 2 * dx;
        //        int d = delta_A - dy;
        //        int delta_B = delta_A - 2 * dy;
        //        for (int i = 0; i < dy; i++)
        //        {
        //            x = next_x;
        //            y = next_y;
        //            if (d > 0)
        //            {
        //                d += delta_B;
        //                next_y += inc_y;
        //                z += inc_z_directiony;
        //                next_x += inc_x;
        //                z += inc_z_directionx;
        //                //_log.warn("3: next_x:"+next_x+" next_y"+next_y);
        //                if (!nLOS(x, y, (int)z, inc_x, inc_y, tz, true))
        //                    return false;
        //            }
        //            else
        //            {
        //                d += delta_A;
        //                next_y += inc_y;
        //                //_log.warn("4: next_x:"+next_x+" next_y"+next_y);
        //                z += inc_z_directiony;
        //                if (!nLOS(x, y, (int)z, 0, inc_y, tz, true))
        //                    return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        private static short sign(int x)
        {
            if (x >= 0)
                return +1;
            else
                return -1;
        }

        private void nInitGeodata()
        {
            try
            {
                logger.Information("Geo Engine: - Loading Geodata...");
                //TODO: Тут пытался подгрузить файл ./data/geodata/geo_index.txt в котором предположительно хранится информация об локациях которые используются в игре в виде 15_25
            }
            catch (Exception e)
            {
                logger.Fatal("Не удалось загрузить Geo Engine");
                throw;
            }

            try
            {
                foreach (var filesInfo in GeoDateFileInfos)
                {
                    loadGeodataFile(filesInfo);
                }
            }
            catch (Exception e)
            {
                logger.Error("Failed to Read geo_index File.", e);
                throw;
            }
        }

        //public static bool unloadGeodata(byte rx, byte ry)
        //{
        //    short regionoffset = (short)((rx << 5) + ry);

        //    try
        //    {
        //        _geodataIndex.remove(regionoffset);
        //        _geodata.remove(regionoffset);
        //        Geodata_files.get(regionoffset).close();
        //        Geodata_files.remove(regionoffset);

        //        _log.info("Geo Engine: - File: " + rx + "_" + ry + ".l2j successfully unloaded.");
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}

        public bool loadGeodataFile(GeoDateFileInfo geoDateFileInfo)
        {
            string fname = "./data/geodata/" + geoDateFileInfo.PositionX + "_" + geoDateFileInfo.PositionY + ".l2j";
            string path = Path.GetFullPath(fname);
            short regionoffset = (short)((geoDateFileInfo.PositionX << 5) + geoDateFileInfo.PositionY);
            logger.Information("GeoEngine: Загружается {@geoDateFileInfo}->region offset:" + regionoffset, geoDateFileInfo);

            //TODO:Тут надо бы обернуть в try catch
            using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
            {
                var accesor = mmf.CreateViewAccessor();
                logger.Information("GeoEngine: Загрузился {@geoDateFileInfo}->Capacity {1}", geoDateFileInfo, accesor.Capacity);
                _geodata.Add(regionoffset, accesor);

                if (accesor.Capacity <= 196608)
                    return true;

                int[] intBuffer = new int[65536];

                int block = 0;
                int index = 0;
                int flor = 0;

                while (block < 65536)
                {
                    byte type = accesor.ReadByte(index);
                    intBuffer[block] = index;
                    block++;
                    index++;

                    if (type == 0)
                        index += 2;
                    else if (type == 1)
                        index += 128;
                    else
                    {
                        for (int b = 0; b < 64; b++)
                        {
                            byte layers = accesor.ReadByte(index);
                            index += (layers << 1) + 1;
                            if (layers > flor)
                                flor = layers;
                        }
                    }
                }

                _geodataIndex.Add(regionoffset, intBuffer);

                return true;
            }
        }

        private static short getRegionOffset(int x, int y)
        {
            int rx = x >> 11; // =/(256 * 8)
            int ry = y >> 11;
            return (short)(((rx + 16) << 5) + (ry + 10));
        }

        /**
         * @param pos
         * @return Block Index: 0-255
         */
        private static int getBlock(int geo_pos)
        {
            return (geo_pos >> 3) % 256;
        }

        /**
         * @param pos
         * @return Cell Index: 0-7
         */
        private static int getCell(int geo_pos)
        {
            return geo_pos % 8;
        }

        /**
         * @param x
         * @param y
         * @return Type of geo_block: 0-2
         */
        //private static short nGetType(int x, int y)
        //{
        //    short region = getRegionOffset(x, y);
        //    int blockX = getBlock(x);
        //    int blockY = getBlock(y);
        //    int index = 0;
        //    //Geodata without index - it is just empty so index can be calculated on the fly
        //    if (_geodataIndex.get(region) == null)
        //    {
        //        index = ((blockX << 8) + blockY) * 3;
        //    }
        //    else
        //    {
        //        //Get Index for current block of current geodata region
        //        index = _geodataIndex.get(region).get((blockX << 8) + blockY);
        //    }

        //    //Buffer that Contains current Region GeoData
        //    ByteBuffer geo = _geodata.get(region);
        //    if (geo == null)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("Geo Region - Region Offset: " + region + " dosnt exist!!");
        //        return 0;
        //    }
        //    return geo.get(index);
        //}

        /**
         * @param x
         * @param y
         * @param z
         * @return Nearlest Z
         */
        //private static short nGetHeight(int geox, int geoy, int z)
        //{
        //    short region = getRegionOffset(geox, geoy);
        //    int blockX = getBlock(geox);
        //    int blockY = getBlock(geoy);
        //    int cellX, cellY, index;
        //    //Geodata without index - it is just empty so index can be calculated on the fly
        //    if (_geodataIndex.get(region) == null) index = ((blockX << 8) + blockY) * 3;
        //    //Get Index for current block of current region geodata
        //    else index = _geodataIndex.get(region).get(((blockX << 8)) + (blockY));
        //    //Buffer that Contains current Region GeoData
        //    ByteBuffer geo = _geodata.get(region);
        //    if (geo == null)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("Geo Region - Region Offset: " + region + " dosnt exist!!");
        //        return (short)z;
        //    }
        //    //Read current block type: 0-flat,1-complex,2-multilevel
        //    byte type = geo.get(index);
        //    index++;
        //    if (type == 0)//flat
        //        return geo.getshort(index);
        //    else if (type == 1)//complex
        //    {
        //        cellX = getCell(geox);
        //        cellY = getCell(geoy);
        //        index += ((cellX << 3) + cellY) << 1;
        //        short height = geo.getshort(index);
        //        height = (short)(height & 0x0fff0);
        //        height = (short)(height >> 1); //height / 2
        //        return height;
        //    }
        //    else //multilevel
        //    {
        //        cellX = getCell(geox);
        //        cellY = getCell(geoy);
        //        int offset = (cellX << 3) + cellY;
        //        while (offset > 0)
        //        {
        //            byte lc = geo.get(index);
        //            index += (lc << 1) + 1;
        //            offset--;
        //        }
        //        byte layers = geo.get(index);
        //        index++;
        //        short height = -1;
        //        if (layers <= 0 || layers > 125)
        //        {
        //            _log.warn("Broken geofile (case1), region: " + region + " - invalid layer count: " + layers + " at: " + geox + " " + geoy);
        //            return (short)z;
        //        }
        //        short temph = short.MinValue;
        //        while (layers > 0)
        //        {
        //            height = geo.getshort(index);
        //            height = (short)(height & 0x0fff0);
        //            height = (short)(height >> 1); //height / 2
        //            if ((z - temph) * (z - temph) > (z - height) * (z - height))
        //                temph = height;
        //            layers--;
        //            index += 2;
        //        }
        //        return temph;
        //    }
        //}

        /**
         * @param x
         * @param y
         * @param zmin
         * @param zmax
         * @return Z betwen zmin and zmax
         */
        //private static short nGetSpawnHeight(int geox, int geoy, int zmin, int zmax, int spawnid)
        //{
        //    short region = getRegionOffset(geox, geoy);
        //    int blockX = getBlock(geox);
        //    int blockY = getBlock(geoy);
        //    int cellX, cellY, index;
        //    short temph = short.MinValue;
        //    //Geodata without index - it is just empty so index can be calculated on the fly
        //    if (_geodataIndex.get(region) == null) index = ((blockX << 8) + blockY) * 3;
        //    //Get Index for current block of current region geodata
        //    else index = _geodataIndex.get(region).get(((blockX << 8)) + (blockY));
        //    //Buffer that Contains current Region GeoData
        //    ByteBuffer geo = _geodata.get(region);
        //    if (geo == null)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("Geo Region - Region Offset: " + region + " dosnt exist!!");
        //        return (short)zmin;
        //    }
        //    //Read current block type: 0-flat,1-complex,2-multilevel
        //    byte type = geo.get(index);
        //    index++;
        //    if (type == 0)//flat	    
        //        temph = geo.getshort(index);
        //    else if (type == 1)//complex
        //    {
        //        cellX = getCell(geox);
        //        cellY = getCell(geoy);
        //        index += ((cellX << 3) + cellY) << 1;
        //        short height = geo.getshort(index);
        //        height = (short)(height & 0x0fff0);
        //        height = (short)(height >> 1); //height / 2
        //        temph = height;
        //    }
        //    else//multilevel
        //    {
        //        cellX = getCell(geox);
        //        cellY = getCell(geoy);
        //        short height;
        //        int offset = (cellX << 3) + cellY;
        //        while (offset > 0)
        //        {
        //            byte lc = geo.get(index);
        //            index += (lc << 1) + 1;
        //            offset--;
        //        }
        //        //Read current block type: 0-flat,1-complex,2-multilevel
        //        byte layers = geo.get(index);
        //        index++;
        //        if (layers <= 0 || layers > 125)
        //        {
        //            _log.warn("Broken geofile (case2), region: " + region + " - invalid layer count: " + layers + " at: " + geox + " " + geoy);
        //            return (short)zmin;
        //        }
        //        while (layers > 0)
        //        {
        //            height = geo.getshort(index);
        //            height = (short)(height & 0x0fff0);
        //            height = (short)(height >> 1); //height / 2
        //            if ((zmin - temph) * (zmin - temph) > (zmin - height) * (zmin - height))
        //                temph = height;
        //            layers--;
        //            index += 2;
        //        }
        //        if (temph > zmax + 200 || temph < zmin - 200)
        //        {
        //            if (_log.isDebugEnabled())
        //                _log.warn("SpawnHeight Error - Couldnt find correct layer to spawn NPC - GeoData or Spawnlist Bug!: zmin: " + zmin + " zmax: " + zmax + " value: " + temph + " SpawnId: " + spawnid + " at: " + geox + " : " + geoy);
        //            return (short)zmin;
        //        }
        //    }
        //    if (temph > zmax + 1000 || temph < zmin - 1000)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("SpawnHeight Error - Spawnlist z value is wrong or GeoData error: zmin: " + zmin + " zmax: " + zmax + " value: " + temph + " SpawnId: " + spawnid + " at: " + geox + " : " + geoy);
        //        return (short)zmin;
        //    }
        //    return temph;
        //}


        /**
         * @param x
         * @param y
         * @param z
         * @param inc_x
         * @param inc_y
         * @param tz
         * @return True if Char can see target
         */
        //private static bool nLOS(int x, int y, int z, int inc_x, int inc_y, int tz, bool debug)
        //{
        //    short region = getRegionOffset(x, y);
        //    int blockX = getBlock(x);
        //    int blockY = getBlock(y);
        //    int cellX, cellY;
        //    short NSWE = 0;

        //    int index;
        //    //Geodata without index - it is just empty so index can be calculated on the fly
        //    if (_geodataIndex.get(region) == null) index = ((blockX << 8) + blockY) * 3;
        //    //Get Index for current block of current region geodata
        //    else index = _geodataIndex.get(region).get(((blockX << 8)) + (blockY));
        //    //Buffer that Contains current Region GeoData
        //    ByteBuffer geo = _geodata.get(region);
        //    if (geo == null)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("Geo Region - Region Offset: " + region + " dosnt exist!!");
        //        return true;
        //    }
        //    //Read current block type: 0-flat,1-complex,2-multilevel
        //    byte type = geo.get(index);
        //    index++;
        //    if (type == 0) //flat, movement and sight always possible
        //    {
        //        if (_log.isDebugEnabled()) _log.warn("flatheight:" + geo.getshort(index));
        //        return true;
        //    }
        //    else if (type == 1) //complex
        //    {
        //        cellX = getCell(x);
        //        cellY = getCell(y);
        //        index += ((cellX << 3) + cellY) << 1;
        //        short height = geo.getshort(index);
        //        NSWE = (short)(height & 0x0F);
        //        height = (short)(height & 0x0fff0);
        //        height = (short)(height >> 1); //height / 2
        //        if (_log.isDebugEnabled())
        //        {
        //            _log.warn("height:" + height + " z" + z);
        //            if (!checkNSWE(NSWE, x, y, x + inc_x, y + inc_y)) _log.warn("would block");
        //        }
        //        if (z - height > 50) return true; // this value is just an approximate
        //    }
        //    else//multilevel, type == 2
        //    {
        //        cellX = getCell(x);
        //        cellY = getCell(y);
        //        int offset = (cellX << 3) + cellY;
        //        while (offset > 0) // iterates (too many times?) to get to layer count
        //        {
        //            byte lc = geo.get(index);
        //            index += (lc << 1) + 1;
        //            offset--;
        //        }
        //        byte layers = geo.get(index);
        //        if (debug) _log.warn("layers" + layers);
        //        index++;
        //        short height = -1;
        //        if (layers <= 0 || layers > 125)
        //        {
        //            _log.warn("Broken geofile (case4), region: " + region + " - invalid layer count: " + layers + " at: " + x + " " + y);
        //            return false;
        //        }
        //        short tempz = short.MinValue; // big negative value
        //        byte temp_layers = layers;
        //        bool highestlayer = true;

        //        z -= 25; // lowering level temporarily to avoid selecting ceiling
        //        while (temp_layers > 0)
        //        {
        //            // reads height for current layer, result in world z coordinate
        //            height = geo.getshort(index);
        //            height = (short)(height & 0x0fff0);
        //            height = (short)(height >> 1); //height / 2
        //                                           //height -= 8; // old geo files had -8 around giran, new data seems better

        //            // searches the closest layer to current z coordinate
        //            if ((z - tempz) * (z - tempz) > (z - height) * (z - height))
        //            {
        //                if (tempz > short.MinValue) highestlayer = false;
        //                tempz = height;
        //                if (debug) _log.warn("z" + (z + 45) + " tempz" + tempz + " dz" + (z - tempz));
        //                NSWE = geo.getshort(index);
        //                NSWE = (short)(NSWE & 0x0F);
        //            }
        //            temp_layers--;
        //            index += 2;
        //        }
        //        z += 25; // level rises back

        //        // Check if LOS goes under a layer/floor  
        //        if ((z - tempz) < -20) return false; // -20 => clearly under, approximates also fence width

        //        // this helps in some cases (occasional under-highest-layer block which isn't wall) 
        //        // but might also create problems in others (passes walls when you're standing high)
        //        if ((z - tempz) > 250) return true;

        //        // or there's a fence/wall ahead when we're not on highest layer
        //        // this part of the check is problematic
        //        if (!highestlayer)
        //        {
        //            //a probable wall, there's movement block and layers above you
        //            if (!checkNSWE(NSWE, x, y, x + inc_x, y + inc_y)) // cannot move
        //            {
        //                // the height after 2 inc_x,inc_y
        //                short nextheight = nGetHeight(x + 2 * inc_x, y + 2 * inc_y, z - 50);
        //                if (_log.isDebugEnabled())
        //                {
        //                    _log.warn("0: z:" + z + " tz" + nGetHeight(x, y, z - 60));
        //                    _log.warn("1: z:" + z + " tz" + nGetHeight(x + inc_x, y + inc_y, z - 60));
        //                    _log.warn("2: z:" + z + " tz" + nGetHeight(x + 2 * inc_x, y + 2 * inc_y, z - 60));
        //                    _log.warn("3: z:" + z + " tz" + nGetHeight(x + 3 * inc_x, y + 3 * inc_y, z - 60));
        //                }
        //                // Probably a very thin fence (e.g. castle fences above artefact),
        //                // where height instantly drops after 1-2 cells and layer ends. 
        //                if (z - nextheight > 100) return true;
        //                // layer continues so close we can see over it
        //                if (nextheight - tempz > 5 && nextheight - tempz < 20) return true;
        //                return false;
        //            }
        //            return true;
        //        }
        //        return true;
        //    }
        //    return checkNSWE(NSWE, x, y, x + inc_x, y + inc_y);
        //}

        /**
         * @param x
         * @param y
         * @param z
         * @return NSWE: 0-15
         */
        //private short nGetNSWE(int x, int y, int z)
        //{
        //    short region = getRegionOffset(x, y);
        //    int blockX = getBlock(x);
        //    int blockY = getBlock(y);
        //    int cellX, cellY;
        //    short NSWE = 0;

        //    int index = 0;
        //    //Geodata without index - it is just empty so index can be calculated on the fly
        //    if (_geodataIndex.get(region) == null) index = ((blockX << 8) + blockY) * 3;
        //    //Get Index for current block of current region geodata
        //    else index = _geodataIndex.get(region).get(((blockX << 8)) + (blockY));
        //    //Buffer that Contains current Region GeoData
        //    ByteBuffer geo = _geodata.get(region);
        //    if (geo == null)
        //    {
        //        if (_log.isDebugEnabled())
        //            _log.warn("Geo Region - Region Offset: " + region + " dosnt exist!!");
        //        return 15;
        //    }
        //    //Read current block type: 0-flat,1-complex,2-multilevel
        //    byte type = geo.get(index);
        //    index++;
        //    if (type == 0)//flat
        //        return 15;
        //    else if (type == 1)//complex
        //    {
        //        cellX = getCell(x);
        //        cellY = getCell(y);
        //        index += ((cellX << 3) + cellY) << 1;
        //        short height = geo.getshort(index);
        //        NSWE = (short)(height & 0x0F);
        //    }
        //    else//multilevel
        //    {
        //        cellX = getCell(x);
        //        cellY = getCell(y);
        //        int offset = (cellX << 3) + cellY;
        //        while (offset > 0)
        //        {
        //            byte lc = geo.get(index);
        //            index += (lc << 1) + 1;
        //            offset--;
        //        }
        //        byte layers = geo.get(index);
        //        index++;
        //        short height = -1;
        //        if (layers <= 0 || layers > 125)
        //        {
        //            _log.warn("Broken geofile (case5), region: " + region + " - invalid layer count: " + layers + " at: " + x + " " + y);
        //            return 15;
        //        }
        //        short tempz = short.MinValue;
        //        while (layers > 0)
        //        {
        //            height = geo.getshort(index);
        //            height = (short)(height & 0x0fff0);
        //            height = (short)(height >> 1); //height / 2

        //            if ((z - tempz) * (z - tempz) > (z - height) * (z - height))
        //            {
        //                tempz = height;
        //                NSWE = geo.get(index);
        //                NSWE = (short)(NSWE & 0x0F);
        //            }
        //            layers--;
        //            index += 2;
        //        }
        //    }
        //    return NSWE;
        //}

        /**
         * @param NSWE
         * @param x
         * @param y
         * @param tx
         * @param ty
         * @return True if NSWE dont block given direction
         */
        private static bool checkNSWE(short NSWE, int x, int y, int tx, int ty)
        {
            //Check NSWE
            if (NSWE == 15)
                return true;
            if (tx > x)//E
            {
                if ((NSWE & _e) == 0)
                    return false;
            }
            else if (tx < x)//W
            {
                if ((NSWE & _w) == 0)
                    return false;
            }
            if (ty > y)//S
            {
                if ((NSWE & _s) == 0)
                    return false;
            }
            else if (ty < y)//N
            {
                if ((NSWE & _n) == 0)
                    return false;
            }
            return true;
        }
    }
}
