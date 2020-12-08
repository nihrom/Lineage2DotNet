using Lineage2.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class WorldSpawnsTest
    {
        public static List<Spawn> Spawns { get; set; }

        public WorldSpawnsTest(Lineage2DbContext dbContext)
        {
            Spawns = dbContext.Spawns.AsNoTracking().ToList();
        }
    }
}
