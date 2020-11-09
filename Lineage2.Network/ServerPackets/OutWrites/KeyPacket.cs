using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network.ServerPackets.OutWrites
{
    public class KeyPacket : OutPacketWriter
    {
        private readonly byte[] _key;

        public KeyPacket(byte[] _key)
        {
            this._key = _key;
        }

        public override void Write()
        {
            WriteByte(0x00);
            WriteByte(0x01);
            WriteBytesArray(_key);
            WriteInt(0x01);
            WriteInt(0x01);
        }
    }
}
