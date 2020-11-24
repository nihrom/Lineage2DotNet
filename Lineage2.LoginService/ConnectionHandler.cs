using L2Crypt;
using Lineage2.Network;
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
            var connection = new L2Connection(tcpClient);
            var loginClient = new LoginClient(scrambledKeyPair, blowfishKey, connection);
            clients.Add(loginClient);
        }
    }
}
