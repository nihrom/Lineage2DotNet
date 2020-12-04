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
        private static readonly ConcurrentDictionary<byte, Action<Packet>> ClientPackets = new ConcurrentDictionary<byte, Action<Packet>>();
        private MainController mainController;

        public ServerPacketHandler(GameClient gameClient)
        {
            mainController = new MainController(gameClient);
            ClientPackets.TryAdd(0x00, mainController.ProtocolVersion);
            ClientPackets.TryAdd(0x08, mainController.AuthLogin);
            ClientPackets.TryAdd(0x0d, mainController.CharacterSelected);
            ClientPackets.TryAdd(0x03, mainController.EnterWorld);
        }

        public void Handle(Packet packet)
        {
            logger.Information($"Получен пакет с Opcode:{packet.FirstOpcode:X2}"); //for State:{client.State}");

            var optocode = packet.FirstOpcode;
            if(ClientPackets.TryGetValue(optocode, out var action))
            {
                action(packet);
            }
            else
            {
                logger.Information($"Для пакета Opcode:{packet.FirstOpcode:X2} нет обработчика");
            }
        }
    }
}
