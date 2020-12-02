using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Lineage2.Repositories
{
    public class DbSeed
    {
        public static async void EnsurePopulated(Lineage2DbContext context)
        {
            context.Database.Migrate();
            
            if (!context.Spawns.Any())
            {

            }

            context.SaveChanges();
        }
    }
}
