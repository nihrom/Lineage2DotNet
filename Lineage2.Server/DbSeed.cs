using Autofac;
using Lineage2.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class DbSeed : IStartable
    {
        readonly Lineage2DbContext context;

        public DbSeed(Lineage2DbContext context)
        {
            this.context = context;
        }

        public async Task EnsurePopulated()
        {
            context.Database.Migrate();

            if (!context.Spawns.Any())
            {

            }

            await context.SaveChangesAsync();
        }

        public void Start()
        {
            EnsurePopulated()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
