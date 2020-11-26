using L2Crypt;
using Lineage2.Network;
using Lineage2.Unility;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Lineage2.Server
{
    public class ConnectionHandler
    {
        private List<GameClient> clients = new List<GameClient>();
        ScrambledKeyPair scrambledKeyPair = new ScrambledKeyPair(ScrambledKeyPair.genKeyPair());
        private byte[] blowfishKey = new byte[16];

        public ConnectionHandler()
        {
            new Random().NextBytes(blowfishKey);
        }

        public void Handle(TcpClient tcpClient)
        {
            byte[] key = BlowFishKeygen.GetRandomKey();
            var crypt = new GameCrypt(key);
            var connection = new L2Connection(tcpClient, crypt);
            var loginClient = new GameClient(connection);
            clients.Add(loginClient);
        }
    }
}
