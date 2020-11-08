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
        public byte[] BlowfishKey { get; private set; }
        public int SessionId { get; private set; }
        public SessionKey SessionKey { get; private set; }
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private readonly LoginCrypt loginCrypt;

        private readonly ILogger logger = Log.Logger.ForContext<LoginClient>();
        IEnumerator<Packet> ie;

        public LoginClient(TcpClient tcpClient, ScrambledKeyPair scrambledKeyPair, byte[] blowfishKey)
        {
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
            Random rnd = new Random();
            SessionId = rnd.Next();
            SessionKey = new SessionKey(rnd.Next(), rnd.Next(), rnd.Next(), rnd.Next());

            RsaPair = scrambledKeyPair;
            BlowfishKey = blowfishKey;

            loginCrypt = new LoginCrypt();
            loginCrypt.UpdateKey(blowfishKey);

            Task.Factory.StartNew(ReadAsync);

            ie = GetPacketAction().GetEnumerator();
            ie.MoveNext();

            _ = Task.Factory.StartNew(() => SendAsync(ie.Current));
        }

        public async Task SendAsync(Packet p)
        {
            byte[] data = p.GetBuffer();
            data = loginCrypt.Encrypt(data, 0, data.Length);

            byte[] lengthBytes = BitConverter.GetBytes((short)(data.Length + 2));
            byte[] message = new byte[data.Length + 2];

            lengthBytes.CopyTo(message, 0);
            data.CopyTo(message, 2);

            await networkStream.WriteAsync(message, 0, message.Length);
            await networkStream.FlushAsync();
        }

        public async Task ReadAsync()
        {
            try
            {
                while (true)
                {
                    short bodyLength = await ReadBodyLengthAsync();

                    byte[] buffer = new byte[bodyLength];
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, bodyLength);

                    if (bytesRead != bodyLength)
                    {
                        throw new Exception("Пакет имеет поврежденную структуру");
                    }

                    if (!loginCrypt.Decrypt(ref buffer, 0, buffer.Length))
                    {
                        throw new Exception($"Blowfish ошибочен для {tcpClient.Client.RemoteEndPoint}.");
                    }

                    var packet = new Packet(1, buffer);

                    logger.Information($"Received packet with Opcode:{packet.FirstOpcode:X2}"); //for State:{client.State}");

                    if (ie.MoveNext())
                    {
                        logger.Information($"Логин сервер отправялет пакет:{ie.Current.FirstOpcode:X2}");
                        _ = Task.Factory.StartNew(() => SendAsync(ie.Current));
                    }

                    //Task.Factory.StartNew(() => _packetHandler.Handle(new Packet(1, buffer), this));
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка: {ex.Message}");
                //TODO: Тут надо новерно закрывать соединение
            }
        }

        private async Task<short> ReadBodyLengthAsync()
        {
            byte[] buffer = new byte[2];
            int bytesRead = await networkStream.ReadAsync(buffer, 0, 2);

            if (bytesRead != 2)
            {
                throw new Exception("Пакет имеет поврежденную структуру");
            }

            short length = BitConverter.ToInt16(buffer, 0);

            return (short)(length - 2);
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
