using L2Crypt;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Lineage2.LoginService
{
    public class ConnectionHandler
    {
        private List<LoginClient> clients = new List<LoginClient>();
        ScrambledKeyPair scrambledKeyPair = new ScrambledKeyPair(ScrambledKeyPair.genKeyPair());
        private byte[] blowfishKey = new byte[16];

        public ConnectionHandler()
        {
            new Random().NextBytes(blowfishKey);
        }

        public void Handle(TcpClient tcpClient)
        {
            var loginClient = new LoginClient(tcpClient, scrambledKeyPair, blowfishKey);
            clients.Add(loginClient);
        }
    }
}
