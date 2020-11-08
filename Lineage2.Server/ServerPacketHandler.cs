using Lineage2.Network;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Server
{
    public class ServerPacketHandler
    {
        private readonly ILogger logger = Log.Logger.ForContext<ServerPacketHandler>();
        private static readonly ConcurrentDictionary<byte, Type> ClientPackets = new ConcurrentDictionary<byte, Type>();

        public ServerPacketHandler()
        {
            //ClientPackets.TryAdd(0x00, typeof(ProtocolVersion));
        }

        public void Handle(Packet packet)
        {
            logger.Information($"Получен пакет с Opcode:{packet.FirstOpcode:X2}"); //for State:{client.State}");
        }
    }
}
