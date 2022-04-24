using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Engine.Factories
{
    public class IdsProdiver : IIdsProdiver
    {
        private readonly object idLock = new object();
        private long lastId;

        public long GetFreeId()
        {
            lock (idLock)
            {
                return lastId++;
            }
        }
    }
}
