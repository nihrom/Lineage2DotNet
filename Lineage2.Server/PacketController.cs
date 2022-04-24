using Lineage2.Engine;
using Lineage2.Engine.User.Controllers;
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
    public class PacketController
    {
        private readonly ILogger logger = Log.Logger.ForContext<PacketController>();
        private readonly MainController mainController;

        private int keyOk;

        public PacketController(MainController mainController)
        {
            this.mainController = mainController;
        }

        public async Task ProtocolVersion(Packet packet)
        {
            int _protocol = packet.ReadInt();
            await mainController.ProtocolVersion(_protocol);
        }

        public async Task Logout(Packet packet)
        {
            await mainController.Logout();
        }

        public async Task AuthLogin(Packet packet)
        {
            var _loginName = packet.ReadString();
            var _playKey2 = packet.ReadInt();
            var _playKey1 = packet.ReadInt();
            var _loginKey1 = packet.ReadInt();
            var _loginKey2 = packet.ReadInt();

            await mainController.AuthLogin(_loginName, _playKey1, _playKey2, _loginKey1, _loginKey2);
        }

        public async Task CharacterSelected(Packet packet)
        {
            int _charSlot = packet.ReadInt();
            int _unk1 = packet.ReadShort();
            int _unk2 = packet.ReadInt();
            int _unk3 = packet.ReadInt();
            int _unk4 = packet.ReadInt();

            await mainController.CharacterSelect(_charSlot, _unk1, _unk2, _unk3, _unk4);
        }

        public async Task EnterWorld(Packet packet)
        {
             await mainController.EnterWorld();
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

            await mainController.MoveBackwardToLocation(current, destination);
        }

        public async Task ExSendManorList(Packet packet)
        {
            await mainController.ExSendManorList();
        }

        public async Task RequestShowMiniMap(Packet packet)
        {
            await mainController.RequestShowMiniMap();
        }
    }
}
