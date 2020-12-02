using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Repositories
{
    public class Lineage2DbContext : DbContext
    {
        public DbSet<Spawn> Spawns { get; set; }

        public Lineage2DbContext(DbContextOptions<Lineage2DbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
