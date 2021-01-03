using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class NodeLoc : AbstractNodeLoc
    {
        private int _x;
        private int _y;
        private short _z;

        public NodeLoc(int x, int y, short z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public override int getX()
        {
            return _x;
        }

        public override int getY()
        {
            return _y;
        }

        public override short getZ()
        {
            return _z;
        }

        public override short getNodeX()
        {
            return 0;
        }

        public override short getNodeY()
        {
            return 0;
        }
    }
}
