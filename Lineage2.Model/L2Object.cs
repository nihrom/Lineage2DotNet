using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class L2Object
    {
        public int ObjId { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual int Z { get; set; }
        public virtual byte Level { get; set; } = 1;
    }
}
