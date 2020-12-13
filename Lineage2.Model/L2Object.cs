using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class L2Object
    {
        public int ObjId { get; set; }
        public Vector3 Position { get; set; }
        public virtual byte Level { get; set; } = 1;
    }
}
