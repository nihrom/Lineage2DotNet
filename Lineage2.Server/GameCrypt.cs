using Lineage2.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Server
{
    public class GameCrypt : INetworkCrypt
    {
        private readonly byte[] inputKey = new byte[16];
        private readonly byte[] outputKey = new byte[16];
        private bool _isEnabled;

        public GameCrypt(byte[] blowfishKey)
        {
            SetKey(blowfishKey);
        }

        public byte[] BlowfishKey => inputKey;

        public void SetKey(byte[] key)
        {
            key.CopyTo(inputKey, 0);
            key.CopyTo(outputKey, 0);
        }

        public void EnableCrypt()
        {
            _isEnabled = true;
        }

        public void Decrypt(byte[] raw)
        {
            if (!_isEnabled)
                return;

            uint num1 = 0;
            for (int index = 0; index < raw.Length; ++index)
            {
                uint num2 = raw[index] & (uint)byte.MaxValue;
                raw[index] = (byte)(num2 ^ inputKey[index & 15] ^ num1);
                num1 = num2;
            }

            uint num3 = ((inputKey[8] & (uint)byte.MaxValue) | (uint)((inputKey[9] << 8) & 65280) | (uint)((inputKey[10] << 16) & 16711680) | (uint)((inputKey[11] << 24) & -16777216)) + (uint)raw.Length;
            inputKey[8] = (byte)(num3 & byte.MaxValue);
            inputKey[9] = (byte)((num3 >> 8) & byte.MaxValue);
            inputKey[10] = (byte)((num3 >> 16) & byte.MaxValue);
            inputKey[11] = (byte)((num3 >> 24) & byte.MaxValue);
        }

        public void Encrypt(byte[] raw)
        {
            if (_isEnabled)
            {
                uint num1 = 0;
                for (int index = 0; index < raw.Length; ++index)
                {
                    num1 = (raw[index] & (uint)byte.MaxValue) ^ outputKey[index & 15] ^ num1;
                    raw[index] = (byte)num1;
                }

                uint num2 = ((outputKey[8] & (uint)byte.MaxValue) | (uint)((outputKey[9] << 8) & 65280) | (uint)((outputKey[10] << 16) & 16711680) | (uint)((outputKey[11] << 24) & -16777216)) + (uint)raw.Length;
                outputKey[8] = (byte)(num2 & byte.MaxValue);
                outputKey[9] = (byte)((num2 >> 8) & byte.MaxValue);
                outputKey[10] = (byte)((num2 >> 16) & byte.MaxValue);
                outputKey[11] = (byte)((num2 >> 24) & byte.MaxValue);
            }
        }
    }
}
