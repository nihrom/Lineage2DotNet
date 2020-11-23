using Lineage2.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Server
{
    public class GameCrypt : INetworkCrypt
    {
        private readonly byte[] _inkey = new byte[16];
        private readonly byte[] _outkey = new byte[16];
        private bool _isEnabled;

        public byte[] BlowfishKey => throw new NotImplementedException();

        public void SetKey(byte[] key)
        {
            key.CopyTo(_inkey, 0);
            key.CopyTo(_outkey, 0);
        }

        public void EnableCrypt(byte[] blowfishKey)
        {
            SetKey(blowfishKey);
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
                raw[index] = (byte)(num2 ^ _inkey[index & 15] ^ num1);
                num1 = num2;
            }

            uint num3 = ((_inkey[8] & (uint)byte.MaxValue) | (uint)((_inkey[9] << 8) & 65280) | (uint)((_inkey[10] << 16) & 16711680) | (uint)((_inkey[11] << 24) & -16777216)) + (uint)raw.Length;
            _inkey[8] = (byte)(num3 & byte.MaxValue);
            _inkey[9] = (byte)((num3 >> 8) & byte.MaxValue);
            _inkey[10] = (byte)((num3 >> 16) & byte.MaxValue);
            _inkey[11] = (byte)((num3 >> 24) & byte.MaxValue);
        }

        public void Encrypt(byte[] raw)
        {
            if (_isEnabled)
            {
                uint num1 = 0;
                for (int index = 0; index < raw.Length; ++index)
                {
                    num1 = (raw[index] & (uint)byte.MaxValue) ^ _outkey[index & 15] ^ num1;
                    raw[index] = (byte)num1;
                }

                uint num2 = ((_outkey[8] & (uint)byte.MaxValue) | (uint)((_outkey[9] << 8) & 65280) | (uint)((_outkey[10] << 16) & 16711680) | (uint)((_outkey[11] << 24) & -16777216)) + (uint)raw.Length;
                _outkey[8] = (byte)(num2 & byte.MaxValue);
                _outkey[9] = (byte)((num2 >> 8) & byte.MaxValue);
                _outkey[10] = (byte)((num2 >> 16) & byte.MaxValue);
                _outkey[11] = (byte)((num2 >> 24) & byte.MaxValue);
            }
        }
    }
}
