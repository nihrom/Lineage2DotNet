using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lineage2.Model.GeoEngine.GeoData
{
    public class BlockComplex : ABlock
    {
        protected byte[] _buffer;

        /**
		 * Implicit constructor for children class.
		 */
        protected BlockComplex()
        {
            // buffer is initialized in children class
            _buffer = null;
        }

        /**
		 * Creates ComplexBlock.
		 * @param bb : Input byte buffer.
		 * @param format : GeoFormat specifying format of loaded data.
		 */
        public BlockComplex(BinaryReader bb, GeoFormat format)
        {
            // initialize buffer
            _buffer = new byte[GeoStructure.BlockCells * 3];

            // load data
            for (int i = 0; i < GeoStructure.BlockCells; i++)
            {
                if (format != GeoFormat.L2D)
                {
                    // Get data
                    short data = bb.ReadInt16();

                    // Get nswe
                    _buffer[i * 3] = (byte)(data & 0x000F);

                    // Get height
                    data = (short)((short)(data & 0xFFF0) >> 1);
                    _buffer[i * 3 + 1] = (byte)(data & 0x00FF);
                    _buffer[i * 3 + 2] = (byte)(data >> 8);
                }
                else
                {
                    // Get nswe
                    byte nswe = bb.ReadByte();
                    _buffer[i * 3] = nswe;

                    // Get height
                    short height = bb.ReadInt16();
                    _buffer[i * 3 + 1] = (byte)(height & 0x00FF);
                    _buffer[i * 3 + 2] = (byte)(height >> 8);
                }
            }
        }

        public override bool HasGeoPos()
        {
            return true;
        }

        public override short GetHeightNearest(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            return (short)(_buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8);
        }

        public override short GetHeightNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return GetHeightNearest(geoX, geoY, worldZ);
        }

        public override short GetHeightAbove(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            short height = (short)(_buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8);

            // check and return height
            return height > worldZ ? height : short.MinValue;
        }

        public override short GetHeightBelow(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            short height = (short)(_buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8);

            // check and return height
            return height < worldZ ? height : short.MaxValue;
        }

        public override byte GetNsweNearest(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get nswe
            return _buffer[index];
        }

        public override byte GetNsweNearestOriginal(int geoX, int geoY, int worldZ)
        {
            return GetNsweNearest(geoX, geoY, worldZ);
        }

        public override byte GetNsweAbove(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            int height = _buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8;

            // check height and return nswe
            return height > worldZ ? _buffer[index] : (byte)0;
        }

        public override byte GetNsweBelow(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            int height = _buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8;

            // check height and return nswe
            return height < worldZ ? _buffer[index] : (byte)0;
        }

        public override int GetIndexNearest(int geoX, int geoY, int worldZ)
        {
            return ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;
        }

        public override int GetIndexAbove(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            int height = _buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8;

            // check height and return nswe
            return height > worldZ ? index : -1;
        }

        public override int GetIndexAboveOriginal(int geoX, int geoY, int worldZ)
        {
            return GetIndexAbove(geoX, geoY, worldZ);
        }

        public override int GetIndexBelow(int geoX, int geoY, int worldZ)
        {
            // Get cell index
            int index = ((geoX % GeoStructure.BlockCellsX) * GeoStructure.BlockCellsY + (geoY % GeoStructure.BlockCellsY)) * 3;

            // Get height
            int height = _buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8;

            // check height and return nswe
            return height < worldZ ? index : -1;
        }

        public override int GetIndexBelowOriginal(int geoX, int geoY, int worldZ)
        {
            return GetIndexBelow(geoX, geoY, worldZ);
        }

        public override short GetHeight(int index)
        {
            return (short)(_buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8);
        }

        public override short GetHeightOriginal(int index)
        {
            return (short)(_buffer[index + 1] & 0x00FF | _buffer[index + 2] << 8);
        }

        public override byte GetNswe(int index)
        {
            return _buffer[index];
        }

        public override byte GetNsweOriginal(int index)
        {
            return _buffer[index];
        }

        public override void SetNswe(int index, byte nswe)
        {
            _buffer[index] = nswe;
        }

        public override void SaveBlock(BufferedStream stream)
        {
            // write block type
            stream.WriteByte(GeoStructure.TypeComplexL2D);

            // write block data
            stream.Write(_buffer, 0, GeoStructure.BlockCells * 3);
        }
    }
}
