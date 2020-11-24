using Lineage2.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lineage2.LoginService
{
    public class LoginServicePacketBuilder
    {
        public Packet Init(LoginClient client)
        {
            byte Opcode = 0x00;
            Packet p = new Packet(Opcode);
            p.WriteInt(client.SessionId, 0x0000c621);
            p.WriteByteArray(client.RsaPair._scrambledModulus);
            p.WriteInt(0x29DD954E, 0x77C39CFC, unchecked((int)0x97ADB620), 0x07BDE0F7);
            p.WriteByteArray(client.L2Connection.Crypt.BlowfishKey);
            p.WriteByteArray(new byte[1]);
            return p;
        }

        public Packet LoginFail(LoginFailReason response)
        {
            byte Opcode = 0x01;
            Packet p = new Packet(Opcode);
            p.WriteByte((byte)response);
            return p;
        }

        public Packet LoginOk(LoginClient client)
        {
            byte Opcode = 0x03;
            Packet p = new Packet(Opcode);
            p.WriteInt(client.SessionKey.LoginOkId1, client.SessionKey.LoginOkId2);
            p.WriteIntArray(new int[2]);
            p.WriteInt(0x000003ea);
            p.WriteIntArray(new int[3]);
            p.WriteByteArray(new byte[16]);
            return p;
        }

        public Packet ServerList()
        {
            byte Opcode = 0x04;
            string serverIp = "127.0.0.1";
            byte serverId = 100;
            Packet p = new Packet(Opcode);
            p.WriteByte(1, serverId); //сначала идет количество серверов, потом последний сервер на котором был игрок
            int bits = 0x40;
            if (false) // если сервер в тестовом моде
                bits |= 0x04;
            p.WriteByte(serverId);
            p.WriteByteArray(serverIp.Split('.').Select(x => byte.Parse(x)).ToArray());
            p.WriteInt(7777);
            p.WriteByte(0);
            p.WriteByte(1); // пвп?
            p.WriteShort(1);
            p.WriteShort(101);
            p.WriteByte(1); // статус сервера
            p.WriteInt(bits);
            p.WriteByte(0); //brackets

            return p;
        }

        public Packet PlayFail(LoginClient client, PlayFailReason reason)
        {
            byte Opcode = 0x06;
            Packet p = new Packet(Opcode);
            p.WriteInt((byte)reason);
            return p;
        }

        public Packet PlayOk(LoginClient client)
        {
            byte Opcode = 0x07;
            Packet p = new Packet(Opcode);
            p.WriteInt(client.SessionKey.PlayOkId1, client.SessionKey.PlayOkId2);
            return p;
        }

        public Packet GGAuth(LoginClient client)
        {
            byte Opcode = 0x0b;
            Packet p = new Packet(Opcode);
            p.WriteInt(client.SessionId);
            p.WriteByteArray(new byte[4]);
            return p;
        }
    }
}
