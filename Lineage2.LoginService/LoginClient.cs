using L2Crypt;
using Lineage2.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.LoginService
{
    public class LoginClient
    {
        public ScrambledKeyPair RsaPair { get; private set; }
        public int SessionId { get; private set; }
        public SessionKey SessionKey { get; private set; }
        public L2Connection L2Connection { get; private set; }

        private readonly ILogger logger = Log.Logger.ForContext<LoginClient>();
        IEnumerator<Packet> ie;

        public LoginClient(ScrambledKeyPair scrambledKeyPair, byte[] blowfishKey, L2Connection l2Connection)
        {
            this.L2Connection = l2Connection;
            Random rnd = new Random();
            SessionId = rnd.Next();
            SessionKey = new SessionKey(rnd.Next(), rnd.Next(), rnd.Next(), rnd.Next());

            RsaPair = scrambledKeyPair;

            ie = GetPacketAction().GetEnumerator();
            ie.MoveNext();

            _ = Task.Factory.StartNew(() => SendAsync(ie.Current));
            L2Connection.ReceivedPacket += LoggingReceivedPacket;
            L2Connection.SendingPacket += LoggingSendPacket;
            l2Connection.ReceivedPacket += OnReadAsync;
            l2Connection.Crypt.EnableCrypt();
        }

        public Task SendAsync(Packet p)
        {
            return L2Connection.SendAsync(p);
        }

        public void OnReadAsync(Packet p)
        {
            if (ie.MoveNext())
            {
                _ = Task.Factory.StartNew(() => SendAsync(ie.Current));
            }
        }

        private void LoggingSendPacket(Packet p)
        {
            logger.Information($"Логин сервер отправялет пакет:{p.FirstOpcode:X2}");
        }

        private void LoggingReceivedPacket(Packet p)
        {
            logger.Information($"Received packet with Opcode:{p.FirstOpcode:X2}");
        }

        public IEnumerable<Packet> GetPacketAction()
        {
            var builder = new LoginServicePacketBuilder();
            yield return builder.Init(this);
            yield return builder.GGAuth(this);
            yield return builder.LoginOk(this);
            yield return builder.ServerList();
            yield return builder.PlayOk(this);
        }
    }
}
