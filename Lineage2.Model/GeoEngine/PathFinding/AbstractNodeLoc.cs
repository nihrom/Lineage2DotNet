using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public abstract class AbstractNodeLoc
    {
        public abstract int getX();
        public abstract int getY();
        public abstract short getZ();
        public abstract short getNodeX();
        public abstract short getNodeY();
    }
}
