using Lineage2.Engine;
using Lineage2.Model;
using Lineage2.Network;
using Lineage2.Network.ServerPackets.OutWrites;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    class MainController
    {
        private readonly ILogger logger = Log.Logger.ForContext<MainController>();
        private readonly GameClient gameClient;
        private readonly GameServerPacketBuilder packetBuilder = new GameServerPacketBuilder();

        private int keyOk;

        public MainController(GameClient gameClient)
        {
            this.gameClient = gameClient;
        }

        public async Task ProtocolVersion(Packet packet)
        {
            int _protocol = packet.ReadInt();

            logger.Information("Протокол соединения: {0}", _protocol);
            var key = gameClient.L2Connection.Crypt.BlowfishKey;
            var packetSend = packetBuilder.CryptInit(key);
            //TODO: Тут хорошо бы сначала дождаться отправки, а потом только включить шифрование
            await gameClient.SendAsync(packetSend);
            gameClient.L2Connection.Crypt.EnableCrypt();
        }

        public async Task Logout(Packet packet)
        {
            var packetSend = packetBuilder.Logout();
            _ = gameClient.SendAsync(packetSend);
        }

        public async Task AuthLogin(Packet packet)
        {
            var _loginName = packet.ReadString();
            var _playKey2 = packet.ReadInt();
            var _playKey1 = packet.ReadInt();
            var _loginKey1 = packet.ReadInt();
            var _loginKey2 = packet.ReadInt();

            keyOk = _playKey1;

            logger.Information("AuthLogin с неймом - {0} и ключами - {1},{2},{3},{4}", _loginName, _playKey2, _playKey1, _loginKey1, _loginKey2);
            var packetSend = packetBuilder.CharList(_playKey1);
            _ = gameClient.SendAsync(packetSend);
        }

        public async Task CharacterSelected(Packet packet)
        {
            int _charSlot = packet.ReadInt();
            int _unk1 = packet.ReadShort();
            int _unk2 = packet.ReadInt();
            int _unk3 = packet.ReadInt();
            int _unk4 = packet.ReadInt();

            var paketSend = packetBuilder.CharacterSelected(keyOk);

            _ = gameClient.SendAsync(paketSend);
        }

        public async Task EnterWorld(Packet packet)
        {
            var packetSend = packetBuilder.UserInfo();
            await gameClient.SendAsync(packetSend);
            int sendNpcInfoCounter = 0;
            foreach (var npc in WorldLauncher.L2Npcs.Where(s => s.Position.x < -56693 + 1000 && s.Position.x > -56693 -1000 && s.Position.y < -113610 + 1000 && s.Position.y > -113610 - 1000))
            {
                sendNpcInfoCounter++;
                var packetSend2 = packetBuilder.NpcInfo(npc);
                await gameClient.SendAsync(packetSend2);
                if (sendNpcInfoCounter >= 50)
                    break;
            }

            logger.Information("Отправлено {0} NpcInfo", sendNpcInfoCounter);
        }

        public async Task MoveBackwardToLocation(Packet packet)
        {
            int _targetX = packet.ReadInt();
            int _targetY = packet.ReadInt();
            int _targetZ = packet.ReadInt();
            int _originX = packet.ReadInt();
            int _originY = packet.ReadInt();
            int _originZ = packet.ReadInt();
            int _moveMovement = packet.ReadInt();

            Vector3 current = new Vector3(_originX, _originY, _originZ);
            Vector3 destination = new Vector3(_targetX, _targetY, _targetZ);

            logger.Information($"CurrentPostition {"x:" + current.x + ", y:" + current.y + ", z:" + current.z}, destinationPosition {"x:" + destination.x + ", y:" + destination.y + ", z:" + destination.z}");

            var sendPacket = packetBuilder.CharMoveToLocation(current, destination);

            await gameClient.SendAsync(sendPacket);
        }

        public async Task ExSendManorList(Packet packet)
        {
            var packetSend = packetBuilder.ExSendManorList();
            await gameClient.SendAsync(packetSend);
        }

        public async Task RequestShowMiniMap(Packet packet)
        {
            var packetSend = packetBuilder.ShowMiniMap();
            await gameClient.SendAsync(packetSend);
        }
    }
}
