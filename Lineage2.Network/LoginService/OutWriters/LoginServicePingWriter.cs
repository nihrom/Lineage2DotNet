using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network.LoginService.OutWriters
{
    public class LoginServicePingWriter : OutPacketWriter
    {
        private readonly string Version;
        private readonly int Build;

        public LoginServicePingWriter(string version, int build)
        {
            Version = version;
            Build = build;
        }

        public override void Write()
        {
            WriteByte(0xA0);
            WriteString(Version);
            WriteInt(Build);
        }
    }
}
