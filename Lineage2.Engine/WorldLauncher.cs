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
        public List<Spawn> Spawns { get; set; }
        public Dictionary<int, NpcTemplate> NpcTemplates { get; set; }
        public List<L2Npc> L2Npcs { get; set; }

        private readonly Factories.NpcFactory npcFactoryCreator;

        public WorldLauncher(ISpawnsRepository spawnsRepository, Factories.NpcFactory npcFactory)
        {
            Spawns = spawnsRepository.GetSpawns();
            this.npcFactoryCreator = npcFactory;
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
                    var npc = npcFactoryCreator.CreateNpc(template, new Vector3(spawn.LocX, spawn.LocY, spawn.LocZ));

                    L2Npcs.Add(npc);
                }
            };
        }
    }
}
