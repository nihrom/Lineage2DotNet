using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model
{
    public class CharTemplate
    {
        public int BaseStr { get; }
        public int BaseCon { get; }
        public int BaseDex { get; }
        public int BaseInt { get; }
        public int BaseWit { get; }
        public int BaseMen { get; }

        private readonly double _baseHpMax;
        private readonly double _baseMpMax;

        public virtual double BaseHpMax(int level) => _baseHpMax;
        public virtual double BaseMpMax(int level) => _baseMpMax;

        public double BaseHpReg { get; }
        public double BaseMpReg { get; }

        public double BasePAtk { get; }
        public double BaseMAtk { get; }
        public double BasePDef { get; }
        public double BaseMDef { get; }

        public int BasePAtkSpd { get; }

        public int BaseCritRate { get; }

        public int BaseWalkSpd { get; }
        public int BaseRunSpd { get; }

        public double CollisionRadius { get; }
        public double CollisionHeight { get; }

        public CharTemplate()
        {
            BaseStr = 40;
            BaseCon = 21;
            BaseDex = 30;
            BaseInt = 20;
            BaseWit = 43;
            BaseMen = 20;

            _baseHpMax = 1000;
            _baseMpMax = 700;

            BaseHpReg = 1.5d;
            BaseMpReg = 0.9d;

            BasePAtk = 10;
            BaseMAtk = 10;
            BasePDef = 10;
            BaseMDef = 10;

            BasePAtkSpd = 300;

            BaseCritRate = 4;

            BaseWalkSpd = 250;
            BaseRunSpd = 300;

            CollisionRadius = 1;
            CollisionHeight = 2;
        }
    }
}
