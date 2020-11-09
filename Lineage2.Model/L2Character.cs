using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class L2Character : L2Object
    {
        public virtual string Name { get; set; }
        public virtual string Title { get; set; }
    }
}
