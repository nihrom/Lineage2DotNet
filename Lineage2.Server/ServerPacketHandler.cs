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
        private MainController mainController;

        public ServerPacketHandler(GameClient gameClient)
        {
            mainController = new MainController(gameClient);
            ClientPackets.TryAdd(0x00, mainController.ProtocolVersion);
            ClientPackets.TryAdd(0x08, mainController.AuthLogin);
            ClientPackets.TryAdd(0x0d, mainController.CharacterSelected);
            ClientPackets.TryAdd(0x03, mainController.EnterWorld);
            ClientPackets.TryAdd(0x01, mainController.MoveBackwardToLocation);

            ClientPacketsD0.TryAdd(0x08, mainController.ExSendManorList);
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
