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

                p.WriteInt(100000); //SP
                p.WriteLong(100); //XP

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
    }
}
