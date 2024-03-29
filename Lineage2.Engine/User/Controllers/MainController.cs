﻿using Lineage2.Engine.Repositories;
using Lineage2.Engine.User;
using Lineage2.Model;
using Lineage2.Model.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Engine.User.Controllers
{
    public class MainController
    {
        private readonly ILogger logger = Log.Logger.ForContext<MainController>();
        private UserAvatar UserAvatar { get; set; }

        //IL2PlayersRepository playersRepository;

        private L2Player L2Player { get; set; }

        private readonly WorldLauncher worldLauncher;
        
        private int keyOk;

        public MainController(WorldLauncher worldLauncher, UserAvatar userAvatar)
        {
            //this.playersRepository = playersRepository;
            this.worldLauncher = worldLauncher;
            this.UserAvatar = userAvatar;

            L2Player = new L2Player()
            {
                CharTemplate = new CharTemplate(),
                AccountName = "Nihrom",
                ClassId = ClassId.OrcFighter,
                Face = Face.TypeB,
                HairColor = HairColor.TypeB,
                HairStyleId = HairStyleId.TypeD,
                Sex = Gender.Male,
                Name = "OrkNagibator",
                Position = new Vector3(-56693, -113610, -691),
                ObjId = 255
            };
        }

        public async Task ProtocolVersion(int protocol)
        {
            logger.Information("Протокол соединения: {Protocol}", protocol);

            await UserAvatar.MainOutput.ProtocolSend();
        }

        public Task ValidatePosition()
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task AuthLogin(string loginName, int playKey1, int playKey2, int loginKey1, int loginKey2)
        {
            logger.Information(
                "AuthLogin с loginName - {LoginName} и ключами - {PlayKey1}, {PlayKey2}, {LoginKey1}, {LoginKey2}",
                loginName,
                playKey1,
                playKey2,
                loginKey1,
                loginKey2);

            keyOk = playKey1;

            var players = new List<L2Player> { L2Player };

            await UserAvatar.MainOutput.SendAccountCharList(players, keyOk);
        }

        public async Task EnterWorld()
        {
            //var player = playersRepository.Get2Player();

            await UserAvatar.MainOutput.UserInfo(L2Player);

            var npcs = worldLauncher.L2Npcs
                .Where(s => 
                    s.Position.x < -56693 + 1000 &&
                    s.Position.x > -56693 - 1000 &&
                    s.Position.y < -113610 + 1000 &&
                    s.Position.y > -113610 - 1000)
                .Take(50)
                .ToList();

            await UserAvatar.MainOutput.NpcInfo(npcs);

            logger.Information("Отправлено {Count} NpcInfo", npcs.Count);
        }

        public async Task RequestAction(int objectId, Vector3 position, byte actionId)
        {
            var npc = worldLauncher.L2Npcs
                .FirstOrDefault(npc => npc.ObjId == objectId);

            await UserAvatar.MainOutput.MyTargetSelected(npc);
            await UserAvatar.MainOutput.StatusUpdate(npc);
        }

        public async Task CharacterSelect(int charSlot, int unk1, int unk2, int unk3, int unk4)
        {
            await UserAvatar.MainOutput.AccountCharacterSelected(L2Player, keyOk);
        }

        public async Task Logout()
        {
            await UserAvatar.MainOutput.Logout();
        }

        public async Task MoveBackwardToLocation(Vector3 current, Vector3 destination)
        {
            logger.Information(
                "CurrentPosition: {@Current}, destinationPosition {@Destination}",
                current,
                destination);

            await UserAvatar.MainOutput.MoveBackwardToLocation(L2Player, current, destination);
        }

        public async Task ExSendManorList()
        {
            await UserAvatar.MainOutput.ExSendManorList();
        }

        public async Task RequestShowMiniMap()
        {
            await UserAvatar.MainOutput.RequestShowMiniMap();
        }
    }
}
