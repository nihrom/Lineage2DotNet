using Lineage2.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class L2Player : L2Character
    {
        public CharTemplate CharTemplate { get; set; }       
        public string AccountName { get; set; }
        public ClassId ClassId { get; set; }

        public Gender Sex { get; set; }
        public HairStyleId HairStyleId { get; set; }
        public HairColor HairColor { get; set; }
        public Face Face { get; set; }
    }
}
