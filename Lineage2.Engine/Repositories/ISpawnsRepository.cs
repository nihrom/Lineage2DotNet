using Lineage2.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Engine.Repositories
{
    public interface ISpawnsRepository
    {
        List<Spawn> GetSpawns();
    }
}
