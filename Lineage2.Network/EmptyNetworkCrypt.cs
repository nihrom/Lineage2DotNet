using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network
{
    public class EmptyNetworkCrypt : INetworkCrypt
    {
        public void Decrypt(byte[] arr)
        {
            return;
        }

        public void Encrypt(byte[] raw)
        {
            return;
        }
    }
}
