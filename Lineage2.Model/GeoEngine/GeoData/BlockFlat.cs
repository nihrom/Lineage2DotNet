using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lineage2.Model.GeoEngine.GeoData
{
    public class BlockFlat : ABlock
    {
        protected short _height;
        protected byte _nswe;

        /**
		 * Creates FlatBlock.
		 * @param bb : Input byte buffer.
		 * @param format : GeoFormat specifying format of loaded data.
		 */
        public BlockFlat(BinaryReader bb, GeoFormat format)
        {
            _height = bb.ReadInt16();
            _nswe = format != GeoFormat.L2D ? (byte)0x0F : (byte)0xFF;

            if (format == GeoFormat.L2OFF)
                bb.ReadInt16();
        }

        public override bool HasGeoPos()
        {
            return true;
        }

        public override short GetHeightNearest(int geoX, int geoY, int worldZ)
        {
            return _height;
        }

        public override short GetHeightNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return _height;
        }

        public override short GetHeightAbove(int geoX, int geoY, int worldZ)
        {
            // check and return height
            return _height > worldZ ? _height : short.MinValue;
        }

        public override short GetHeightBelow(int geoX, int geoY, int worldZ)
        {
            // check and return height
            return _height < worldZ ? _height : short.MaxValue;
        }

        public override byte GetNsweNearest(int geoX, int geoY, int worldZ)
        {
            return _nswe;
        }

        public override byte GetNsweNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return _nswe;
        }

        public override byte GetNsweAbove(int geoX, int geoY, int worldZ)
        {
            // check height and return nswe
            return _height > worldZ ? _nswe : (byte)0;
        }

        public override byte GetNsweBelow(int geoX, int geoY, int worldZ)
        {
            // check height and return nswe
            return _height < worldZ ? _nswe : (byte)0;
        }

        public override int GetIndexNearest(int geoX, int geoY, int worldZ)
        {
            return 0;
        }

        public override int GetIndexAbove(int geoX, int geoY, int worldZ)
        {
            // check height and return index
            return _height > worldZ ? 0 : -1;
        }

        public override int GetIndexAboveOriginal(int geoX, int geoY, int worldZ)
        {
            return GetIndexAbove(geoX, geoY, worldZ);
        }

        public override int GetIndexBelow(int geoX, int geoY, int worldZ)
        {
            // check height and return index
            return _height < worldZ ? 0 : -1;
        }

        public override int GetIndexBelowOriginal(int geoX, int geoY, int worldZ)
        {
            return GetIndexBelow(geoX, geoY, worldZ);
        }

        public override short GetHeight(int index)
        {
            return _height;
        }

        public override short GetHeightOriginal(int index)
        {
            return _height;
        }

        public override byte GetNswe(int index)
        {
            return _nswe;
        }

        public override byte GetNsweOriginal(int index)
        {
            return _nswe;
        }

        public override void SetNswe(int index, byte nswe)
        {
            _nswe = nswe;
        }

        public override void SaveBlock(BufferedStream stream)
        {
            // write block type
            stream.WriteByte(GeoStructure.TypeFlatL2D);

            // write height
            stream.WriteByte((byte)(_height & 0x00FF));
            stream.WriteByte((byte)(_height >> 8));
        }
    }
}
