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

            logger.Information("AuthLogin с неймом - {0} и ключами - {1},{2},{3},{4}", _loginName, _playKey2, _playKey1, _loginKey1, _loginKey2);
            var packetSend = packetBuilder.CharList(_playKey1);
            _ = gameClient.SendAsync(packetSend);
        }
    }
}
