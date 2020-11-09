using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Unility
{
    //TODO: Удалить этот класс в случае, если на самом деле нет необходимости создавать ключ, который содержит не рандомные участки
    public class BlowFishKeygen
    {
        private const int CryptKeysSize = 20;
        private static byte[][] CryptKeys;
        private static readonly Random Random = new Random();

        public static void GenerateKeys()
        {
            CryptKeys = new byte[CryptKeysSize][];
            for (int i = 0; i < CryptKeysSize; i++)
            {
                CryptKeys[i] = new byte[16];

                for (int j = 0; j < CryptKeys[i].Length; j++)
                    CryptKeys[i][j] = (byte)Random.Next(255);

                CryptKeys[i][8] = 0xc8;
                CryptKeys[i][9] = 0x27;
                CryptKeys[i][10] = 0x93;
                CryptKeys[i][11] = 0x01;
                CryptKeys[i][12] = 0xa1;
                CryptKeys[i][13] = 0x6c;
                CryptKeys[i][14] = 0x31;
                CryptKeys[i][15] = 0x97;
            }
        }

        public static byte[] GetRandomKey()
        {
            if (CryptKeys == null)
                GenerateKeys();

            return CryptKeys[Random.Next(CryptKeysSize)];
        }
    }
}
