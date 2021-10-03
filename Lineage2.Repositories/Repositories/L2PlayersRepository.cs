using Lineage2.Engine.Repositories;
using Lineage2.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Database.Repositories
{
    public class L2PlayersRepository : IL2PlayersRepository
    {
        Lineage2DbContext db;

        public L2PlayersRepository(Lineage2DbContext db)
        {
            this.db = db;
        }

        public L2Player Get2Player()
        {
            throw new NotImplementedException();
        }
    }
}
