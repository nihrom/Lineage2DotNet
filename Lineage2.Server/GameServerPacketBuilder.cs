using Lineage2.Model;
using Lineage2.Model.Enums;
using Lineage2.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Server
{
    class GameServerPacketBuilder
    {
        public Packet CryptInit(byte[] key)
        {
            byte opcode = 0x00;
            Packet p = new Packet(opcode);
            p.WriteByte(0x01);
            p.WriteByteArray(key);
            p.WriteInt(0x01);
            p.WriteInt(0x01);

            return p;
        }

        public Packet CharList(int sessionKeyPlayOk1)
        {
            byte opcode = 0x13;
            Packet p = new Packet(opcode);
            p.WriteInt(1); //количесво чаров

            int lastSelectedObjId = 0;

            L2Player l2Player = new L2Player()
            {
                CharTemplate = new CharTemplate(),
                AccountName = "Nihrom",
                ClassId = ClassId.OrcFighter,
                Face = Face.TypeB,
                HairColor = HairColor.TypeB,
                HairStyleId = HairStyleId.TypeD,
                Sex = Gender.Male,
                Name = "OrkNagibator",
                ObjId = 255
            };

            List<L2Player> players = new List<L2Player>();
            players.Add(l2Player);

            //if (_players.Count > 0)
            //{
            //    if (_players.Count(filter => filter.DeleteTime == 0) == 1)
            //        lastSelectedObjId = _players.FirstOrDefault().ObjId;
            //    else
            //        lastSelectedObjId = _players.OrderByDescending(sort => sort.LastAccess).FirstOrDefault(filter => filter.DeleteTime == 0).ObjId;
            //}

            foreach (L2Player player in players)
            {
                p.WriteString(player.Name);
                p.WriteInt(player.ObjId);
                p.WriteString(player.AccountName);
                p.WriteInt(sessionKeyPlayOk1);
                p.WriteInt(0);//player.ClanId
                p.WriteInt(0x00); // ??

                p.WriteInt((int)player.Sex);
                p.WriteInt((int)player.ClassId.ClassRace);

                if (true)//player.ActiveClass.ClassId.Id == player.BaseClass.ClassId.Id) //тут похоже выбирает какой класс активен сейчас у пользователя
                    p.WriteInt((int)player.ClassId.Id);
                //else
                //    p.WriteInt((int)player.BaseClass.ClassId.Id);

                p.WriteInt(0x01); // active ??

                p.WriteInt(-56693);//player.X);
                p.WriteInt(-113610);//player.Y);
                p.WriteInt(-690);//player.Z);

                p.WriteDouble((double)1000);//player.CharStatus.CurrentHp);
                p.WriteDouble((double)700);//player.CharStatus.CurrentMp);

                p.WriteInt(1000); //SP
                p.WriteLong(10); //XP

                p.WriteInt(1);//player.Level);
                p.WriteInt(0); //Karma
                p.WriteInt(0); //PkKills
                p.WriteInt(0); //PvpKills

                p.WriteInt(0);
                p.WriteInt(0);
                p.WriteInt(0);
                p.WriteInt(0);
                p.WriteInt(0);
                p.WriteInt(0);
                p.WriteInt(0);

                for (byte id = 0; id < 17; id++) //Inventory.PaperdollTotalslots; id++)
                    p.WriteInt(0); //player.Inventory.Paperdoll[id]?.item?.ItemId ?? 0);

                for (byte id = 0; id < 17; id++)//Inventory.PaperdollTotalslots; id++)
                    p.WriteInt(0); //player.Inventory.Paperdoll[id]?.item?.ItemId ?? 0);

                p.WriteInt((int)player.HairStyleId);
                p.WriteInt((int)player.HairColor);

                p.WriteInt((int)player.Face);
                p.WriteDouble(player.CharTemplate.BaseHpMax(1));
                p.WriteDouble(player.CharTemplate.BaseMpMax(1));
                p.WriteInt(0); //RemainingDeleteTime

                p.WriteInt((int)player.ClassId.Id);

                int selection;

                //if (CharId != -1)
                //    selection = CharId == player.ObjId ? 1 : 0;

                //if ((lastSelectedObjId > 0) && (lastSelectedObjId == player.ObjId))
                //    selection = 1;

                selection = 1;

                p.WriteInt(selection); // auto-select char
                p.WriteByte(0); //GetEnchantValue
                p.WriteInt(0x00); // augment
            }

            return p;
        }

        public Packet CharacterSelected(int sessionKeyPlayOk1)
        {
            byte opcode = 0x15;
            Packet p = new Packet(opcode);

            L2Player _player = new L2Player()
            {
                CharTemplate = new CharTemplate(),
                AccountName = "Nihrom",
                ClassId = ClassId.OrcFighter,
                Face = Face.TypeB,
                HairColor = HairColor.TypeB,
                HairStyleId = HairStyleId.TypeD,
                Sex = Gender.Male,
                Name = "OrkNagibator",
                ObjId = 255
            };

            p.WriteString(_player.Name);
            p.WriteInt(_player.ObjId);
            p.WriteString(_player.Title);
            p.WriteInt(sessionKeyPlayOk1);

            p.WriteInt(0);//_player.ClanId
            p.WriteInt(0x00); //??
            p.WriteInt((int)_player.Sex);
            p.WriteInt((int)_player.ClassId.ClassRace);

            p.WriteInt((int)_player.ClassId.Id);
            p.WriteInt(0x01); // active ??
            p.WriteInt(-56693);//player.X);
            p.WriteInt(-113610);//player.Y);
            p.WriteInt(-690);//player.Z);
            p.WriteDouble(1000);
            p.WriteDouble(700);
            p.WriteInt(1000);

            p.WriteLong(0);
            p.WriteInt(1);
            p.WriteInt(0);
            p.WriteInt(0); //?

            p.WriteInt(_player.CharTemplate.BaseInt);
            p.WriteInt(_player.CharTemplate.BaseStr);
            p.WriteInt(_player.CharTemplate.BaseCon);
            p.WriteInt(_player.CharTemplate.BaseMen);
            p.WriteInt(_player.CharTemplate.BaseDex);
            p.WriteInt(_player.CharTemplate.BaseWit);

            for (int i = 0; i < 30; i++)
                p.WriteInt(0x00);

            p.WriteInt(0x00); // c3 work
            p.WriteInt(0x00); // c3 work

            p.WriteInt(0);

            p.WriteInt(0x00); // c3

            p.WriteInt((int)_player.ClassId.Id);

            p.WriteInt(0x00); // c3 InspectorBin
            p.WriteInt(0x00); // c3
            p.WriteInt(0x00); // c3
            p.WriteInt(0x00); // c3

            return p;
        }

        public Packet UserInfo()
        {
            byte opcode = 0x04;
            Packet p = new Packet(opcode);

            L2Player _player = new L2Player()
            {
                CharTemplate = new CharTemplate(),
                AccountName = "Nihrom",
                ClassId = ClassId.OrcFighter,
                Face = Face.TypeB,
                HairColor = HairColor.TypeB,
                HairStyleId = HairStyleId.TypeD,
                Sex = Gender.Male,
                Name = "OrkNagibator",
                ObjId = 255
            };


            p.WriteInt(-56693);//player.X);
            p.WriteInt(-113610);//player.Y);
            p.WriteInt(-691);//player.Z);
            p.WriteInt(28); //_player.Heading
            p.WriteInt(_player.ObjId);

            p.WriteString(_player.Name);

            p.WriteInt((int)_player.ClassId.ClassRace);
            p.WriteInt((int)_player.Sex);
            p.WriteInt((int)_player.ClassId.Id);
            p.WriteInt(_player.Level);
            p.WriteLong(10);

            p.WriteInt(_player.CharTemplate.BaseStr);
            p.WriteInt(_player.CharTemplate.BaseDex);
            p.WriteInt(_player.CharTemplate.BaseCon);
            p.WriteInt(_player.CharTemplate.BaseInt);
            p.WriteInt(_player.CharTemplate.BaseWit);
            p.WriteInt(_player.CharTemplate.BaseMen);

            p.WriteInt(1000); //max hp
            p.WriteInt(1000);
            p.WriteInt(700); //max mp
            p.WriteInt(700);
            p.WriteInt(1000);
            p.WriteInt(20);//_player.CurrentWeight
            p.WriteInt(100);

            p.WriteInt(20); //_player.Inventory.GetPaperdollItem(Inventory.PaperdollRhand) != null ? 40 : 20); // 20 no weapon, 40 weapon equipped

            for (byte id = 0; id < 17; id++)
                p.WriteInt(0);

            for (byte id = 0; id < 17; id++)
                p.WriteInt(0);

            // c6 new h's
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteInt(0x00); //player.Inventory.getPaperdollAugmentId(InvPC.EQUIPITEM_RHand));
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteInt(0x00); //player.Inventory.getPaperdollAugmentId(InvPC.EQUIPITEM_LHand));
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);
            p.WriteShort(0x00);

            p.WriteInt((int)_player.CharTemplate.BasePAtk);
            p.WriteInt(300);
            p.WriteInt(100);
            p.WriteInt(2);
            p.WriteInt(2);
            p.WriteInt(10);
            p.WriteInt(50);
            p.WriteInt(150);
            p.WriteInt(300); //? еще раз?
            p.WriteInt(100);

            p.WriteInt(0);
            p.WriteInt(0);

            p.WriteInt(200);
            p.WriteInt(180);
            p.WriteInt(50); // swimspeed
            p.WriteInt(50); // swimspeed
            p.WriteInt(0); //?
            p.WriteInt(0); //?
            p.WriteInt(200);
            p.WriteInt(180);
            p.WriteDouble(1); //run speed multiplier
            p.WriteDouble(1); //atk speed multiplier

            p.WriteDouble(11);
            p.WriteDouble(28);

            p.WriteInt((int)_player.HairStyleId);
            p.WriteInt((int)_player.HairColor);
            p.WriteInt((int)_player.Face);
            p.WriteInt(1);

            p.WriteString(_player.Title);

            p.WriteInt(0);//_player.ClanId
            p.WriteInt(0);//_player.ClanCrestId
            p.WriteInt(0);//_player.AllianceId
            p.WriteInt(0);//_player.AllianceCrestId

            p.WriteInt(0); //_relation
            p.WriteByte(0);
            p.WriteByte(0); //
            p.WriteByte(0);
            p.WriteInt(0);
            p.WriteInt(0);

            p.WriteShort(0);//_player.Cubics.Count

            //_player.Cubics.ForEach(cub =>p.WriteShort(cub.Template.Id));

            p.WriteByte(0); //1-isInPartyMatchRoom

            p.WriteInt(0);

            //byte flymode = 0;

            //if (player.TransformID > 0)
            //    flymode = player.Transform.Template.MoveMode;

            p.WriteByte(0x00);

            p.WriteInt(0);//_player.ClanPrivs

            p.WriteShort(0); //c2  recommendations remaining
            p.WriteShort(0); //c2  recommendations received
            p.WriteInt(0); //moun t npcid
            p.WriteShort(80);

            p.WriteInt((int)_player.ClassId.Id);
            p.WriteInt(0); // special effects? circles around player...
            p.WriteInt(100); //max cp
            p.WriteInt(100);
            p.WriteByte(0);
            p.WriteByte(0);
            p.WriteInt(0);//_player.GetClanCrestLargeId()
            p.WriteByte(0);

            //byte hero = _player.Heroic;
            p.WriteByte(0);

            p.WriteByte(0x00); //Fishing Mode
            p.WriteInt(0);//_player.GetFishx()); //fishing x
            p.WriteInt(0);//_player.GetFishy()); //fishing y
            p.WriteInt(0);//_player.GetFishz()); //fishing z
            p.WriteInt(0xFFFFFF);

            p.WriteByte(1);//_player.IsRunning);

            p.WriteInt(0);//_player.ClanRank()
            p.WriteInt(0);//_player.ClanType);

            p.WriteInt(0xFFFF77);//_player.GetTitleColor());
            p.WriteInt(0);

            return p;
        }

        public Packet ExSendManorList()
        {
            List<string> manorsName = new List<string>
                {
                    "gludio",
                    "dion",
                    "giran",
                    "oren",
                    "aden",
                    "innadril",
                    "goddard",
                    "rune",
                    "schuttgart"
                };

            byte opcode = 0xFE;
            Packet p = new Packet(opcode);
            p.WriteShort(0x1B);
            p.WriteInt(manorsName.Count);

            int id = 1;
            foreach (string manor in manorsName)
            {
                p.WriteInt(id);
                id++;
                p.WriteString(manor);
            }

            return p;
        }

        public Packet NpcInfo(L2Npc npc)
        {
            byte opcode = 0x16;
            Packet p = new Packet(opcode);

            p.WriteInt(npc.ObjId);
            p.WriteInt(npc.NpcTemplate.NpcId + 1000000);
            p.WriteInt(0);
            p.WriteInt((int)npc.Position.x);
            p.WriteInt((int)npc.Position.y);
            p.WriteInt((int)npc.Position.z);
            p.WriteInt(0);
            p.WriteInt(0x00);

            double spd = 150;//_npc.CharacterStat.GetStat(EffectType.PSpeed);
            double atkspd = 1200;//_npc.CharacterStat.GetStat(EffectType.BAttackSpd);
            double cast = 1200;//_npc.CharacterStat.GetStat(EffectType.BCastingSpd);
            double anim = (spd * 1f) / 120;
            double anim2 = (1.1 * atkspd) / 277;

            p.WriteInt((int)cast);
            p.WriteInt((int)atkspd);
            p.WriteInt((int)spd);
            p.WriteInt((int)(spd * .8));
            p.WriteInt(0); // swimspeed
            p.WriteInt(0); // swimspeed
            p.WriteInt(0);
            p.WriteInt(0);
            p.WriteInt(0);
            p.WriteInt(0);

            p.WriteDouble(anim);
            p.WriteDouble(anim2);
            p.WriteDouble(npc.NpcTemplate.CollisionRadius);
            p.WriteDouble(npc.NpcTemplate.CollisionHeight);
            p.WriteInt(0); // right hand weapon
            p.WriteInt(0);
            p.WriteInt(0); // left hand weapon
            p.WriteByte(1); // name above char 1=true ... ??
            p.WriteByte(0);
            p.WriteByte(0);
            p.WriteByte(0);
            p.WriteByte(0); // invisible ?? 0=false  1=true   2=summoned (only works if model has a summon animation)
            p.WriteString(npc.Name);
            p.WriteString(npc.Title);
            p.WriteInt(0x00); // Title color 0=client default
            p.WriteInt(0x00); //pvp flag
            p.WriteInt(0x00); // karma

            p.WriteInt(0);
            p.WriteInt(0);//_npc.ClanId
            p.WriteInt(0);//_npc.ClanCrestId
            p.WriteInt(0);//_npc.AllianceId
            p.WriteInt(0);//_npc.AllianceCrestId
            p.WriteByte(0); // C2

            p.WriteByte(0);
            p.WriteDouble(13);
            p.WriteDouble(11.5);
            p.WriteInt(0); // enchant
            p.WriteInt(0); // C6

            return p;
        }
    }
}
