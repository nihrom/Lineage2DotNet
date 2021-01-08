using Lineage2.Model.GeoEngine.GeoData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class GeoNodeLoc : AbstractNodeLoc
    {
        private short _x;
        private short _y;
        private short _z;

        public GeoNodeLoc(short x, short y, short z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public override int getX()
        {
            return GeoStructure.WorldXMin + _x * 128 + 48;
        }

        public override int getY()
        {
            return GeoStructure.WorldYMin + _y * 128 + 48;
        }

        public override short getZ()
        {
            return _z;
        }

        public override short getNodeX()
        {
            return _x;
        }

        public override short getNodeY()
        {
            return _y;
        }
    }
}
