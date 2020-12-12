using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.Items
{
    public class DropData
    {
        public int ItemId { get; set; }
        public int MinDrop { get; set; }
        public int MaxDrop { get; set; }
        public int Chance { get; set; }
    }
}
