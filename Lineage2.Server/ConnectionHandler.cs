using L2Crypt;
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
            var loginClient = new GameClient(tcpClient, scrambledKeyPair, blowfishKey);
            clients.Add(loginClient);
        }
    }
}
