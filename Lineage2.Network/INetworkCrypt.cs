using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Network
{
    public interface INetworkCrypt
    {
        void Decrypt(byte[] arr);
        void Encrypt(byte[] raw);
    }
}
