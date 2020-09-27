using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network.LoginService.OutWriters
{
    public class ServerInfoWriter : OutPacketWriter
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string AuthCode { get; set; }
        public bool IsGmOnly { get; set; }
        public bool IsTestServer { get; set; }
        public int MaxPlayers { get; set; }

        public override void Write()
        {
            WriteByte(0xA1);
            WriteShort(Port);
            WriteString(Host);
            WriteString(string.Empty);
            WriteString(AuthCode);
            WriteInt(0);
            WriteShort(MaxPlayers);
            WriteByte(IsGmOnly ? 0x01 : 0x00);
            WriteByte(IsTestServer ? 0x01 : 0x00);
        }
    }
}
