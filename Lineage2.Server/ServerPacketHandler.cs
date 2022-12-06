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

        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPackets = new();
        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPacketsD0 = new();

        public ServerPacketHandler(PacketController packetController)
        {
            clientPackets.TryAdd(0x00, packetController.ProtocolVersion);
            clientPackets.TryAdd(0x01, packetController.MoveBackwardToLocation);
            clientPackets.TryAdd(0x03, packetController.EnterWorld);
            clientPackets.TryAdd(0x04, packetController.RequestAction);
            clientPackets.TryAdd(0x08, packetController.AuthLogin);
            clientPackets.TryAdd(0x09, packetController.Logout);
            clientPackets.TryAdd(0x0d, packetController.CharacterSelected);
            clientPackets.TryAdd(0x48, packetController.ValidatePosition);
            clientPackets.TryAdd(0xcd, packetController.RequestShowMiniMap);

            clientPacketsD0.TryAdd(0x08, packetController.ExSendManorList);
        }

        public async Task Handle(Packet packet)
        {
            logger.Information(
                "Получен пакет с Opcode:{FirstOpcode:X2}",
                packet.FirstOpcode);

            var opcode = packet.FirstOpcode;

            if(opcode != 0xD0)
            {
                if (clientPackets.TryGetValue(opcode, out var action))
                {
                    await action(packet);
                }
                else
                {
                    logger.Error(
                        "Для пакета Opcode:{FirstOpcode:X2} нет обработчика",
                        packet.FirstOpcode);
                }
            }
            else
            {
                var opcodeD0 = (byte)packet.SecondOpcode;

                if (clientPacketsD0.TryGetValue(opcodeD0, out var action))
                {
                    await action(packet);
                }
                else
                {
                    logger.Error(
                        "Для пакета D0 c SecondOpcode:{SecondOpcode:X2} нет обработчика",
                        packet.SecondOpcode);
                }
            }
        }
    }
}
