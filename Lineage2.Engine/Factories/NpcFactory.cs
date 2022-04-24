using Lineage2.Model;
using Lineage2.Model.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Engine.Factories
{
    public class NpcFactory
    {
        private readonly IIdsProdiver idsProdiver;

        public NpcFactory(IIdsProdiver idsProdiver)
        {
            this.idsProdiver = idsProdiver;
        }

        public L2Npc CreateNpc(NpcTemplate npcTemplate, Vector3 position)
        {
            return new L2Npc()
            {
                ObjId = (int)idsProdiver.GetFreeId(),
                NpcTemplate = npcTemplate,
                Position = position
            };
        }
    }
}
