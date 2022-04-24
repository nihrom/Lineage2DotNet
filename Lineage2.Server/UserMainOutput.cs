using Lineage2.Engine.User.Output;
using Lineage2.Model;
using Lineage2.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class UserMainOutput : IMainOutput
    {
        GameClient gameClient;

        public UserMainOutput(GameClient gameClient)
        {
            this.gameClient = gameClient;
        }

        public async Task ProtocolSend()
        {
            var key = gameClient.L2Connection.Crypt.BlowfishKey;
            var packetSend = GameServerPacketBuilder.ProtocolResponse(key);

            await gameClient.SendAsync(packetSend);

            gameClient.L2Connection.Crypt.EnableCrypt();
        }

        public async Task AccountCharacterSelected(L2Player l2Player, int keyOk)
        {
            var packet = GameServerPacketBuilder.CharacterSelected(l2Player, keyOk);

            await gameClient.SendAsync(packet);
        }

        public async Task SendAccountCharList(List<L2Player> l2Players, int playKey1)
        {
            var packet = GameServerPacketBuilder.CharList(l2Players, playKey1);

            await gameClient.SendAsync(packet);
        }

        public async Task Logout()
        {
            var packetSend = GameServerPacketBuilder.Logout();

            await gameClient.SendAsync(packetSend);
        }

        public async Task MoveBackwardToLocation(L2Object l2Object, Vector3 current, Vector3 destination)
        {
            var packet = GameServerPacketBuilder.CharMoveToLocation(l2Object, current, destination);

            await gameClient.SendAsync(packet);
        }

        public async Task ExSendManorList()
        {
            var packet = GameServerPacketBuilder.ExSendManorList();

            await gameClient.SendAsync(packet);
        }

        public async Task RequestShowMiniMap()
        {
            var packet = GameServerPacketBuilder.ShowMiniMap();

            await gameClient.SendAsync(packet);
        }

        public async Task UserInfo(L2Player l2Player)
        {
            var packet = GameServerPacketBuilder.UserInfo(l2Player);

            await gameClient.SendAsync(packet);
        }

        public async Task NpcInfo(List<L2Npc> npcs)
        {
            foreach (var npc in npcs)
            {
                var packet = GameServerPacketBuilder.NpcInfo(npc);

                await gameClient.SendAsync(packet);
            }
        }
    }
}
