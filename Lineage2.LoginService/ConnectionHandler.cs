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


        public ConnectionHandler()
        {
            
        }

        public void Handle(TcpClient tcpClient)
        {
            byte[] blowfishKey = new byte[16];
            new Random().NextBytes(blowfishKey);
            var crypt = new LoginCrypt(blowfishKey);
            var connection = new L2Connection(tcpClient, crypt);
            var loginClient = new LoginClient(scrambledKeyPair, blowfishKey, connection);
            clients.Add(loginClient);
        }
    }
}
