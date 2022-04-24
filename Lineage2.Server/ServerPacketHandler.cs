using Lineage2.Network;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class ServerPacketHandler
    {
        private readonly ILogger logger = Log.Logger.ForContext<ServerPacketHandler>();
        private static readonly ConcurrentDictionary<byte, Func<Packet, Task>> ClientPackets = new ConcurrentDictionary<byte, Func<Packet, Task>>();
        private static readonly ConcurrentDictionary<byte, Func<Packet, Task>> ClientPacketsD0 = new ConcurrentDictionary<byte, Func<Packet, Task>>();

        public ServerPacketHandler(PacketController packetController)
        {
            ClientPackets.TryAdd(0x00, packetController.ProtocolVersion);
            ClientPackets.TryAdd(0x01, packetController.MoveBackwardToLocation);
            ClientPackets.TryAdd(0x03, packetController.EnterWorld);
            ClientPackets.TryAdd(0x08, packetController.AuthLogin);
            ClientPackets.TryAdd(0x09, packetController.Logout);
            ClientPackets.TryAdd(0x0d, packetController.CharacterSelected);
            ClientPackets.TryAdd(0xcd, packetController.RequestShowMiniMap);

            ClientPacketsD0.TryAdd(0x08, packetController.ExSendManorList);
        }

        public async Task Handle(Packet packet)
        {
            logger.Information($"Получен пакет с Opcode:{packet.FirstOpcode:X2}"); //for State:{client.State}");

            var optocode = packet.FirstOpcode;
            if(optocode != 0xD0)
            {
                if (ClientPackets.TryGetValue(optocode, out var action))
                {
                    await action(packet);
                }
                else
                {
                    logger.Error($"Для пакета Opcode:{packet.FirstOpcode:X2} нет обработчика");
                }
            }
            else
            {
                var optocodeD0 = (byte)packet.SecondOpcode;
                if (ClientPacketsD0.TryGetValue(optocodeD0, out var action))
                {
                    await action(packet);
                }
                else
                {
                    logger.Error($"Для пакета D0 c SecondOpcode:{packet.SecondOpcode:X2} нет обработчика");
                }
            }
        }
    }
}
