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

        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPacketsConnected = new();
        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPacketsAuthed = new();
        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPackets = new();
        private readonly ConcurrentDictionary<byte, Func<Packet, Task>> clientPacketsD0 = new();

        public ServerPacketHandler(PacketController packetController)
        {
            clientPacketsConnected.TryAdd(0x00, packetController.ProtocolVersion);
            clientPacketsConnected.TryAdd(0x08, packetController.AuthLogin);

            clientPacketsAuthed.TryAdd(0x09, packetController.Logout);
            // clientPacketsAuthed.TryAdd(0x0b, packetController.CharacterCreate);
            // clientPacketsAuthed.TryAdd(0x0c, packetController.CharacterDelete);
            clientPacketsAuthed.TryAdd(0x0d, packetController.CharacterSelected);
            // clientPacketsAuthed.TryAdd(0x0e, packetController.NewCharacter);
            // clientPacketsAuthed.TryAdd(0x62, packetController.CharacterRestore);
            // clientPacketsAuthed.TryAdd(0x68, packetController.RequestPledgeCrest);

            clientPackets.TryAdd(0x00, packetController.ProtocolVersion);//TODO: Connected packet?
            clientPackets.TryAdd(0x01, packetController.MoveBackwardToLocation);
            //clientPackets.TryAdd(0x02, ); TODO: ClientPackets 0x02
            clientPackets.TryAdd(0x03, packetController.EnterWorld);
            clientPackets.TryAdd(0x04, packetController.RequestAction);
            clientPackets.TryAdd(0x08, packetController.AuthLogin); //TODO: Connected packet?
            clientPackets.TryAdd(0x09, packetController.Logout);
            //clientPackets.TryAdd(0x0a, packetController.AttackRequest);
            clientPackets.TryAdd(0x0d, packetController.CharacterSelected);  //TODO: Authed packet?
            // clientPackets.TryAdd(0x0f, packetController.RequestItemList);
            // clientPackets.TryAdd(0x10, packetController.RequestEquipItem); //RequestEquipItem ... not used any more, instead "useItem"
            // clientPackets.TryAdd(0x11, packetController.RequestUnEquipItem);
            // clientPackets.TryAdd(0x12, packetController.RequestDropItem);
            // //clientPackets.TryAdd(0x13, );//unknown 
            // clientPackets.TryAdd(0x14, packetController.UseItem);
            // clientPackets.TryAdd(0x15, packetController.TradeRequest);
            // clientPackets.TryAdd(0x16, packetController.AddTradeItem);
            // clientPackets.TryAdd(0x17, packetController.TradeDone);
            // clientPackets.TryAdd(0x1a, packetController.DummyPacket);
            // clientPackets.TryAdd(0x1b, packetController.RequestSocialAction);
            // clientPackets.TryAdd(0x1c, packetController.RequestChangeMoveType);
            // clientPackets.TryAdd(0x1d, packetController.RequestChangeWaitType);
            // clientPackets.TryAdd(0x1e, packetController.RequestSellItem);
            // clientPackets.TryAdd(0x1f, packetController.RequestBuyItem);
            // clientPackets.TryAdd(0x20, packetController.RequestLinkHtml);
            // clientPackets.TryAdd(0x21, packetController.RequestBypassToServer);
            // clientPackets.TryAdd(0x22, packetController.RequestBBSwrite);
            // clientPackets.TryAdd(0x23, packetController.DummyPacket);
            // clientPackets.TryAdd(0x24, packetController.RequestJoinPledge);
            // clientPackets.TryAdd(0x25, packetController.RequestAnswerJoinPledge);
            // clientPackets.TryAdd(0x26, packetController.RequestWithdrawPledge);
            // clientPackets.TryAdd(0x27, packetController.RequestOustPledgeMember);
            // //clientPackets.TryAdd(0x28, ); // unknown RequestDismissPledge
            // clientPackets.TryAdd(0x29, packetController.RequestJoinParty);
            // clientPackets.TryAdd(0x2a, packetController.RequestAnswerJoinParty);
            // clientPackets.TryAdd(0x2b, packetController.RequestWithdrawParty);
            // clientPackets.TryAdd(0x2c, packetController.RequestOustPartyMember);
            // //clientPackets.TryAdd(0x2d, ); // unknown RequestDismissParty
            // clientPackets.TryAdd(0x2e, packetController.DummyPacket);
            // clientPackets.TryAdd(0x2f, packetController.RequestMagicSkillUse);
            // clientPackets.TryAdd(0x30, packetController.Appearing);
            // clientPackets.TryAdd(0x31, packetController.SendWarehouseDepositList); //if (Config.ALLOW_WAREHOUSE)
            // clientPackets.TryAdd(0x32, packetController.SendWarehouseWithdrawList);
            // clientPackets.TryAdd(0x33, packetController.RequestShortCutReg);
            // clientPackets.TryAdd(0x34, packetController.DummyPacket);
            // clientPackets.TryAdd(0x35, packetController.RequestShortCutDel);
            // clientPackets.TryAdd(0x36, packetController.CannotMoveAnymore);
            // clientPackets.TryAdd(0x37, packetController.RequestTargetCanceld);
            // clientPackets.TryAdd(0x38, packetController.Say2);
            // clientPackets.TryAdd(0x3c, packetController.RequestPledgeMemberList);
            // clientPackets.TryAdd(0x3e, packetController.DummyPacket);
            // clientPackets.TryAdd(0x3f, packetController.RequestSkillList);
            // //clientPackets.TryAdd(0x41,); //MoveWithDelta ... unused ?? or only on ship ??
            // clientPackets.TryAdd(0x42, packetController.RequestGetOnVehicle);
            // clientPackets.TryAdd(0x43, packetController.RequestGetOffVehicle);
            // clientPackets.TryAdd(0x44, packetController.AnswerTradeRequest);
            // clientPackets.TryAdd(0x45, packetController.AnswerTradeRequest);
            // clientPackets.TryAdd(0x46, packetController.RequestRestart);
            // //clientPackets.TryAdd(0x47, ); //RequestSiegeInfo
            clientPackets.TryAdd(0x48, packetController.ValidatePosition);
            // //clientPackets.TryAdd(0x49, ); //RequestSEKCustom
            // clientPackets.TryAdd(0x4a, packetController.StartRotating);
            // clientPackets.TryAdd(0x4b, packetController.FinishRotating);
            // clientPackets.TryAdd(0x4d, packetController.RequestStartPledgeWar);
            // clientPackets.TryAdd(0x4e, packetController.RequestReplyStartPledgeWar);
            // clientPackets.TryAdd(0x4f, packetController.RequestStopPledgeWar);
            // clientPackets.TryAdd(0x50, packetController.RequestReplyStopPledgeWar);
            // clientPackets.TryAdd(0x51, packetController.RequestSurrenderPledgeWar);
            // clientPackets.TryAdd(0x52, packetController.RequestReplySurrenderPledgeWar);
            // clientPackets.TryAdd(0x53, packetController.RequestSetPledgeCrest);
            // clientPackets.TryAdd(0x55, packetController.RequestGiveNickName);
            // clientPackets.TryAdd(0x57, packetController.RequestShowBoard);
            // clientPackets.TryAdd(0x58, packetController.RequestEnchantItem);
            // clientPackets.TryAdd(0x59, packetController.RequestDestroyItem);
            // clientPackets.TryAdd(0x5b, packetController.SendBypassBuildCmd);
            // clientPackets.TryAdd(0x5d, packetController.CannotMoveAnymoreInVehicle);
            // clientPackets.TryAdd(0x5e, packetController.RequestFriendInvite);
            // clientPackets.TryAdd(0x5f, packetController.RequestAnswerFriendInvite);
            // clientPackets.TryAdd(0x60, packetController.RequestFriendList);
            // clientPackets.TryAdd(0x61, packetController.RequestFriendDel);
            // clientPackets.TryAdd(0x63, packetController.RequestQuestList);
            // clientPackets.TryAdd(0x64, packetController.RequestQuestAbort);
            // clientPackets.TryAdd(0x66, packetController.RequestPledgeInfo);
            // //clientPackets.TryAdd(0x67, );//RequestPledgeExtendedInfo
            // clientPackets.TryAdd(0x69, packetController.RequestSurrenderPersonally);
            // //clientPackets.TryAdd(0x6a, );//Ride
            // clientPackets.TryAdd(0x6b, packetController.RequestAcquireSkillInfo);
            // clientPackets.TryAdd(0x6c, packetController.RequestAcquireSkill);
            // clientPackets.TryAdd(0x6d, packetController.RequestRestartPoint);
            // clientPackets.TryAdd(0x6e, packetController.RequestGMCommand);
            // clientPackets.TryAdd(0x6f, packetController.RequestPartyMatchConfig);
            // clientPackets.TryAdd(0x70, packetController.RequestPartyMatchList);
            // clientPackets.TryAdd(0x71, packetController.RequestPartyMatchDetail);
            // clientPackets.TryAdd(0x72, packetController.RequestCrystallizeItem);
            // clientPackets.TryAdd(0x73, packetController.RequestPrivateStoreManageSell);
            // clientPackets.TryAdd(0x74, packetController.SetPrivateStoreListSell);
            // //clientPackets.TryAdd(0x75, ); //RequestPrivateStoreManageCancel
            // clientPackets.TryAdd(0x76, packetController.RequestPrivateStoreQuitSell);
            // clientPackets.TryAdd(0x77, packetController.SetPrivateStoreMsgSell);
            // //clientPackets.TryAdd(0x78, ); //RequestPrivateStoreList
            // clientPackets.TryAdd(0x79, packetController.RequestPrivateStoreBuy);
            // //clientPackets.TryAdd(0x7a, ); //ReviveReply
            // clientPackets.TryAdd(0x7b, packetController.RequestTutorialLinkHtml);
            // clientPackets.TryAdd(0x7e, packetController.RequestTutorialClientEvent);
            // clientPackets.TryAdd(0x7f, packetController.RequestPetition);
            // clientPackets.TryAdd(0x80, packetController.RequestPetitionCancel);
            // clientPackets.TryAdd(0x81, packetController.RequestGmList);
            // clientPackets.TryAdd(0x82, packetController.RequestJoinAlly);
            // clientPackets.TryAdd(0x83, packetController.RequestAnswerJoinAlly);
            // clientPackets.TryAdd(0x84, packetController.AllyLeave);
            // clientPackets.TryAdd(0x85, packetController.AllyDismiss);
            // clientPackets.TryAdd(0x86, packetController.RequestDismissAlly);
            // clientPackets.TryAdd(0x87, packetController.RequestSetAllyCrest);
            // clientPackets.TryAdd(0x88, packetController.RequestAllyCrest);
            // clientPackets.TryAdd(0x89, packetController.RequestChangePetName);
            // clientPackets.TryAdd(0x8a, packetController.RequestPetUseItem);
            // clientPackets.TryAdd(0x8b, packetController.RequestGiveItemToPet);
            // clientPackets.TryAdd(0x8c, packetController.RequestGetItemFromPet);
            // clientPackets.TryAdd(0x8e, packetController.RequestAllyInfo);
            // clientPackets.TryAdd(0x8f, packetController.RequestPetGetItem);
            // clientPackets.TryAdd(0x90, packetController.RequestPrivateStoreManageBuy);
            // clientPackets.TryAdd(0x91, packetController.SetPrivateStoreListBuy);
            // //clientPackets.TryAdd(0x92, ); //RequestPrivateStoreBuyManageCancel
            // clientPackets.TryAdd(0x93, packetController.RequestPrivateStoreQuitBuy);
            // clientPackets.TryAdd(0x94, packetController.SetPrivateStoreMsgBuy);
            // //clientPackets.TryAdd(0x95, ); //RequestPrivateStoreBuyList
            // clientPackets.TryAdd(0x96, packetController.RequestPrivateStoreSell);
            // //clientPackets.TryAdd(0x97, ); //SendTimeCheckPacket
            // //clientPackets.TryAdd(0x98, ); //RequestStartAllianceWar
            // //clientPackets.TryAdd(0x99, ); //ReplyStartAllianceWar
            // //clientPackets.TryAdd(0x9a, ); //RequestStopAllianceWar
            // //clientPackets.TryAdd(0x9b, ); //ReplyStopAllianceWar
            // //clientPackets.TryAdd(0x9c, ); //RequestSurrenderAllianceWar
            // //clientPackets.TryAdd(0x9d, ); //RequestSkillCoolTime
            // clientPackets.TryAdd(0x9e, packetController.RequestPackageSendableItemList);
            // clientPackets.TryAdd(0x9f, packetController.RequestPackageSend);
            // clientPackets.TryAdd(0xa0, packetController.RequestBlock);
            // //clientPackets.TryAdd(0xa1, ); //RequestCastleSiegeInfo
            // clientPackets.TryAdd(0xa2, packetController.RequestSiegeAttackerList);
            // clientPackets.TryAdd(0xa3, packetController.RequestSiegeDefenderList);
            // clientPackets.TryAdd(0xa4, packetController.RequestJoinSiege);
            // clientPackets.TryAdd(0xa5, packetController.RequestConfirmSiegeWaitingList);
            // //clientPackets.TryAdd(0xa6, ); //RequestSetCastleSiegeTime
            // clientPackets.TryAdd(0xa7, packetController.MultiSellChoose);
            // //clientPackets.TryAdd(0xa8, ); //NetPing
            // clientPackets.TryAdd(0xaa, packetController.RequestUserCommand);
            // clientPackets.TryAdd(0xab, packetController.SnoopQuit);
            // clientPackets.TryAdd(0xac, packetController.RequestRecipeBookOpen); // we still need this packet to handle BACK button of craft dialog
            // clientPackets.TryAdd(0xad, packetController.RequestRecipeBookDestroy);
            // clientPackets.TryAdd(0xae, packetController.RequestRecipeItemMakeInfo);
            // clientPackets.TryAdd(0xaf, packetController.RequestRecipeItemMakeSelf);
            // //clientPackets.TryAdd(0xb0, ); //RequestRecipeShopManageList
            // clientPackets.TryAdd(0xb1, packetController.RequestRecipeShopMessageSet);
            // clientPackets.TryAdd(0xb2, packetController.RequestRecipeShopListSet);
            // clientPackets.TryAdd(0xb3, packetController.RequestRecipeShopManageQuit);
            // clientPackets.TryAdd(0xb5, packetController.RequestRecipeShopMakeInfo);
            // clientPackets.TryAdd(0xb6, packetController.RequestRecipeShopMakeItem);
            // clientPackets.TryAdd(0xb7, packetController.RequestRecipeShopManagePrev);
            // clientPackets.TryAdd(0xb8, packetController.ObserverReturn);
            // clientPackets.TryAdd(0xb9, packetController.RequestEvaluate);
            // clientPackets.TryAdd(0xba, packetController.RequestHennaList);
            // clientPackets.TryAdd(0xbb, packetController.RequestHennaItemInfo);
            // clientPackets.TryAdd(0xbc, packetController.RequestHennaEquip);
            // clientPackets.TryAdd(0xbd, packetController.RequestHennaRemoveList);
            // clientPackets.TryAdd(0xbe, packetController.RequestHennaItemRemoveInfo);
            // clientPackets.TryAdd(0xbf, packetController.RequestHennaRemove);
            // clientPackets.TryAdd(0xc0, packetController.RequestPledgePower); // Clan Privileges
            // clientPackets.TryAdd(0xc1, packetController.RequestMakeMacro);
            // clientPackets.TryAdd(0xc2, packetController.RequestDeleteMacro);
            // clientPackets.TryAdd(0xc3, packetController.RequestBuyProcure);
            // clientPackets.TryAdd(0xc4, packetController.RequestBuySeed);
            // clientPackets.TryAdd(0xc5, packetController.DlgAnswer);
            // clientPackets.TryAdd(0xc6, packetController.RequestPreviewItem);
            // clientPackets.TryAdd(0xc7, packetController.RequestSSQStatus);
            // clientPackets.TryAdd(0xCA, packetController.GameGuardReply);
            // clientPackets.TryAdd(0xcc, packetController.RequestSendFriendMsg);
            // clientPackets.TryAdd(0xcc, packetController.RequestSendFriendMsg);
            clientPackets.TryAdd(0xcd, packetController.RequestShowMiniMap);
            // //clientPackets.TryAdd(0xce, ); // MSN dialogs so that you dont see them in the console.
            // clientPackets.TryAdd(0xcf, packetController.RequestRecordInfo);

            // clientPacketsD0.TryAdd(0x01, packetController.RequestOustFromPartyRoom);
            // clientPacketsD0.TryAdd(0x02, packetController.RequestDismissPartyRoom);
            // clientPacketsD0.TryAdd(0x03, packetController.RequestWithdrawPartyRoom);
            // clientPacketsD0.TryAdd(0x04, packetController.RequestChangePartyLeader);
            // clientPacketsD0.TryAdd(0x05, packetController.RequestAutoSoulShot);
            // clientPacketsD0.TryAdd(0x06, packetController.RequestExEnchantSkillInfo);
            // clientPacketsD0.TryAdd(0x07, packetController.RequestExEnchantSkill);
            // clientPacketsD0.TryAdd(0x08, packetController.RequestManorList);
            clientPacketsD0.TryAdd(0x09, packetController.ExSendManorList);
            // clientPacketsD0.TryAdd(0x0a, packetController.RequestSetSeed);
            // clientPacketsD0.TryAdd(0x0b, packetController.RequestSetCrop);
            // clientPacketsD0.TryAdd(0x0c, packetController.RequestWriteHeroWords);
            // clientPacketsD0.TryAdd(0x0d, packetController.RequestExAskJoinMPCC);
            // clientPacketsD0.TryAdd(0x0f, packetController.RequestExOustFromMPCC);
            // clientPacketsD0.TryAdd(0x10, packetController.RequestExPledgeCrestLarge);
            // clientPacketsD0.TryAdd(0x11, packetController.RequestExSetPledgeCrestLarge);
            // clientPacketsD0.TryAdd(0x12, packetController.RequestOlympiadObserverEnd);
            // clientPacketsD0.TryAdd(0x13, packetController.RequestOlympiadMatchList);
            // clientPacketsD0.TryAdd(0x14, packetController.RequestAskJoinPartyRoom);
            // clientPacketsD0.TryAdd(0x16, packetController.RequestListPartyMatchingWaitingRoom);
            // clientPacketsD0.TryAdd(0x17, packetController.RequestExitPartyMatchingWaitingRoom);
            // clientPacketsD0.TryAdd(0x18, packetController.RequestGetBossRecord);
            // clientPacketsD0.TryAdd(0x19, packetController.RequestPledgeSetAcademyMaster);
            // clientPacketsD0.TryAdd(0x1a, packetController.RequestPledgePowerGradeList);
            // clientPacketsD0.TryAdd(0x1b, packetController.RequestPledgeMemberPowerInfo);
            // clientPacketsD0.TryAdd(0x1c, packetController.RequestPledgeSetMemberPowerGrade);
            // clientPacketsD0.TryAdd(0x1d, packetController.RequestPledgeMemberInfo);
            // clientPacketsD0.TryAdd(0x1e, packetController.RequestPledgeWarList);
            // clientPacketsD0.TryAdd(0x1f, packetController.RequestExFishRanking);
            // clientPacketsD0.TryAdd(0x20, packetController.RequestPCCafeCouponUse);
            // clientPacketsD0.TryAdd(0x22, packetController.RequestCursedWeaponList);
            // clientPacketsD0.TryAdd(0x23, packetController.RequestCursedWeaponLocation);
            // clientPacketsD0.TryAdd(0x24, packetController.RequestPledgeReorganizeMember);
            // clientPacketsD0.TryAdd(0x26, packetController.RequestExMPCCShowPartyMembersInfo);
            // clientPacketsD0.TryAdd(0x27, packetController.RequestDuelStart);
            // clientPacketsD0.TryAdd(0x28, packetController.RequestDuelAnswerStart);
            // clientPacketsD0.TryAdd(0x2c, packetController.RequestRefine);
            // clientPacketsD0.TryAdd(0x2d, packetController.RequestConfirmCancelItem);
            // clientPacketsD0.TryAdd(0x2e, packetController.RequestRefineCancel);
            // clientPacketsD0.TryAdd(0x2f, packetController.RequestExMagicSkillUseGround);
            // clientPacketsD0.TryAdd(0x30, packetController.RequestDuelSurrender);
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
