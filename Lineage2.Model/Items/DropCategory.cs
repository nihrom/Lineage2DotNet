using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.Items
{
    public class DropCategory
    {
        public List<DropData> Drops { get; private set; }
        public int CategoryChance { get; private set; }
        public int CategoryBalancedChance { get; private set; }
        public int CategoryType { get; private set; }

        public DropCategory(List<DropData> drops, int categoryChance, int categoryBalancedChance, int categoryType)
        {
            Drops = drops;
            CategoryChance = categoryChance;
            CategoryBalancedChance = categoryBalancedChance;
            CategoryType = categoryType;
        }
    }
}
