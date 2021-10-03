using L2Crypt;
using Lineage2.Network;
using Lineage2.Unility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class GameClient
    {
        private readonly ILogger logger = Log.Logger.ForContext<GameClient>();
        public L2Connection L2Connection { get; private set; }
        public readonly ServerPacketHandler packetHandler;

        public GameClient(L2Connection l2Connection)
        {
            L2Connection = l2Connection;
            packetHandler = new ServerPacketHandler(this);
            L2Connection.ReceivedPacket += LoggingReceivedPacket;
            L2Connection.SendingPacket += LoggingSendPacket;
            l2Connection.ReceivedPacket += OnReadAsync;
        }

        public Task SendAsync(Packet p)
        {
            return L2Connection.SendAsync(p);
        }

        public void OnReadAsync(Packet p)
        {
            _ = packetHandler.Handle(p);
        }

        private void LoggingSendPacket(Packet p)
        {
            logger.Information($"Cервер отправялет пакет:{p.FirstOpcode:X2}");
        }

        private void LoggingReceivedPacket(Packet p)
        {
            logger.Information($"Получен пакет с кодом:{p.FirstOpcode:X2}");
        }

        //public byte[] EnableCrypt()
        //{
        //    byte[] key = BlowFishKeygen.GetRandomKey();
        //    var _crypt = new GameCrypt();
        //    _crypt.SetKey(key);
        //    crypt = _crypt;
        //    return key;
        //}
    }
}
