using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.Templates
{
    public class CharTemplate
    {
        public int BaseStr { get; set; }
        public int BaseCon { get; set; }
        public int BaseDex { get; set; }
        public int BaseInt { get; set; }
        public int BaseWit { get; set; }
        public int BaseMen { get; set; }

        public double BaseHpReg { get; set; }
        public double BaseMpReg { get; set; }

        public virtual double BaseHpMax(int level = 1) => 100;
        public virtual double BaseMpMax(int level = 1) => 100;

        public double BasePAtk { get; set; }
        public double BaseMAtk { get; set; }
        public double BasePDef { get; set; }
        public double BaseMDef { get; set; }

        public int BasePAtkSpd { get; set; }

        public int BaseCritRate { get; set; }

        public int BaseWalkSpd { get; set; }
        public int BaseRunSpd { get; set; }

        public double CollisionRadius { get; set; }
        public double CollisionHeight { get; set; }
    }
}
