using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.Templates
{
    public class NpcTemplate : CharTemplate
    {
        public int NpcId { get; set; }
        public int TemplateId { get; set; }
        public byte Level { get; set; }
        public int Exp { get; set; }
        public int Sp { get; set; }
        public int RHand { get; set; }
        public int LHand { get; set; }
        public string Type { get; set; }
        public int CorpseTime { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }
        public int DropHerbGroup { get; set; }
    }
}
