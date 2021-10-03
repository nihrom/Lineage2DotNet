using Lineage2.Engine.Repositories;
using Lineage2.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lineage2.Database.Repositories
{
    public class SpawnsRepository : ISpawnsRepository
    {
        Lineage2DbContext db;

        public SpawnsRepository(Lineage2DbContext db)
        {
            this.db = db;
        }

        public List<Spawn> GetSpawns()
        {
            return db.Spawns
                .AsNoTracking()
                .ToList();
        }
    }
}
