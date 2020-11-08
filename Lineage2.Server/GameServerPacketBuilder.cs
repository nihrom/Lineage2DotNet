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
            p.WriteByte(0x00);
            p.WriteByte(0x01);
            p.WriteByteArray(key);
            p.WriteInt(0x01);
            p.WriteInt(0x01);

            return p;
        }

        public Packet CharList()
        {
            byte opcode = 0x13;
            Packet p = new Packet(opcode);
            p.WriteInt(0); //количесво чаров

            int lastSelectedObjId = 0;

            //if (_players.Count > 0)
            //{
            //    if (_players.Count(filter => filter.DeleteTime == 0) == 1)
            //        lastSelectedObjId = _players.FirstOrDefault().ObjId;
            //    else
            //        lastSelectedObjId = _players.OrderByDescending(sort => sort.LastAccess).FirstOrDefault(filter => filter.DeleteTime == 0).ObjId;
            //}

            //foreach (L2Player player in _players)
            //{
            //    p.WriteString(player.Name);
            //    p.WriteInt(player.ObjId);
            //    p.WriteString(_account);
            //    p.WriteInt(_sessionId);
            //    p.WriteInt(0);//player.ClanId
            //    p.WriteInt(0x00); // ??

            //   p.WriteInt((int)player.Sex);
            //   p.WriteInt((int)player.BaseClass.ClassId.ClassRace);

            //    if (player.ActiveClass.ClassId.Id == player.BaseClass.ClassId.Id)
            //       p.WriteInt((int)player.ActiveClass.ClassId.Id);
            //    else
            //       p.WriteInt((int)player.BaseClass.ClassId.Id);

            //   p.WriteInt(0x01); // active ??

            //   p.WriteInt(player.X);
            //   p.WriteInt(player.Y);
            //   p.WriteInt(player.Z);

            //   p.WriteDouble(player.CharStatus.CurrentHp);
            //   p.WriteDouble(player.CharStatus.CurrentMp);

            //   p.WriteInt(player.Sp);
            //   p.WriteLong(player.Exp);

            //   p.WriteInt(player.Level);
            //   p.WriteInt(player.Karma);
            //   p.WriteInt(player.PkKills);
            //   p.WriteInt(player.PvpKills);

            //   p.WriteInt(0);
            //   p.WriteInt(0);
            //   p.WriteInt(0);
            //   p.WriteInt(0);
            //   p.WriteInt(0);
            //   p.WriteInt(0);
            //   p.WriteInt(0);

            //    for (byte id = 0; id < Inventory.PaperdollTotalslots; id++)
            //       p.WriteInt(player.Inventory.Paperdoll[id]?.item?.ItemId ?? 0);

            //    for (byte id = 0; id < Inventory.PaperdollTotalslots; id++)
            //       p.WriteInt(player.Inventory.Paperdoll[id]?.item?.ItemId ?? 0);

            //   p.WriteInt((int)player.HairStyleId);
            //   p.WriteInt((int)player.HairColor);

            //   p.WriteInt((int)player.Face);
            //   p.WriteDouble(player.MaxHp);
            //   p.WriteDouble(player.MaxMp);
            //   p.WriteInt(player.RemainingDeleteTime());

            //   p.WriteInt((int)player.ActiveClass.ClassId.Id);

            //    int selection = 0;

            //    if (CharId != -1)
            //        selection = CharId == player.ObjId ? 1 : 0;

            //    if ((lastSelectedObjId > 0) && (lastSelectedObjId == player.ObjId))
            //        selection = 1;

            //   p.WriteInt(selection); // auto-select char
            //   p.WriteByte(player.GetEnchantValue());
            //   p.WriteInt(0x00); // augment
            //}

            return p;
        }
    }
}
