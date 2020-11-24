using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network
{
    public class EmptyNetworkCrypt : INetworkCrypt
    {
        public byte[] BlowfishKey => new byte[0];

        public void Decrypt(byte[] arr)
        {
            return;
        }

        public void EnableCrypt(byte[] blowfishKey)
        {
            return;
        }

        public void Encrypt(byte[] raw)
        {
            return;
        }
    }
}
