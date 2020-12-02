﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Repositories
{
    public class Spawn
    {
        public int Id;
        public int SpanwnTemplateId { get; set; }

        public int LocX { get; set; }

        public int LocY { get; set; }

        public int LocZ { get; set; }

        public int Heading { get; set; }

        public int RespawnDelay { get; set; }

        public int RespawnRand { get; set; }

        public int PerdiodOfDay { get; set; }
    }
}
