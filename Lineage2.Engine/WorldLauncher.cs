using Lineage2.Engine.Repositories;
using Lineage2.Model;
using Lineage2.Model.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Engine
{
    public class WorldLauncher
    {
        public static List<Spawn> Spawns { get; set; }
        public static Dictionary<int, NpcTemplate> NpcTemplates { get; set; }
        public static List<L2Npc> L2Npcs { get; set; }
        public static int GlobaL2Ids = 260;

        public WorldLauncher(ISpawnsRepository spawnsRepository)
        {
            Spawns = spawnsRepository.GetSpawns();

            Launche();
        }

        public async Task Launche()
        {
            NpcFactory npcFactory = new NpcFactory();
            NpcTemplates = npcFactory.Initialize();
            L2Npcs = new List<L2Npc>(Spawns.Count);

            foreach (var spawn in Spawns)
            {
                if (NpcTemplates.TryGetValue(spawn.SpanwnTemplateId, out NpcTemplate template))
                {
                    var npc = new L2Npc()
                    {
                        ObjId = GetNextId(),
                        NpcTemplate = template,
                        Position = new Vector3(spawn.LocX, spawn.LocY, spawn.LocZ)
                    };

                    L2Npcs.Add(npc);
                }
            };
        }

        public int GetNextId()
        {
            GlobaL2Ids++;
            return GlobaL2Ids;
        }
    }
}
