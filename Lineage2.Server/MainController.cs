using Lineage2.Network;
using Lineage2.Network.ServerPackets.OutWrites;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void ProtocolVersion(Packet packet)
        {
            int _protocol = packet.ReadInt();

            logger.Information("Протокол соединения: {0}", _protocol);

            var key = gameClient.EnableCrypt();
            var packetSend = packetBuilder.CryptInit(key);

            //var pac = new KeyPacket(key);
            //pac.Write();

            _ = gameClient.SendAsync(packetSend);
        }

        public void AuthLogin(Packet packet)
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

        public void CharacterSelected(Packet packet)
        {
            int _charSlot = packet.ReadInt();
            int _unk1 = packet.ReadShort();
            int _unk2 = packet.ReadInt();
            int _unk3 = packet.ReadInt();
            int _unk4 = packet.ReadInt();

            var paketSend = packetBuilder.CharacterSelected(keyOk);

            _ = gameClient.SendAsync(paketSend);
        }

        public void EnterWorld(Packet packet)
        {
            var packetSend = packetBuilder.UserInfo();
            _ = gameClient.SendAsync(packetSend);
        }
    }
}
