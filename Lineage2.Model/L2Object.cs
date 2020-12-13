using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class L2Object
    {
        public int ObjId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public virtual byte Level { get; set; } = 1;
    }
}
