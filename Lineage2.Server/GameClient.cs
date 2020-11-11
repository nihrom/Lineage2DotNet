using L2Crypt;
using Lineage2.Network;
using Lineage2.Unility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class GameClient
    {
        public ScrambledKeyPair RsaPair { get; private set; }
        public byte[] BlowfishKey { get; private set; }
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private INetworkCrypt crypt;

        private readonly ILogger logger = Log.Logger.ForContext<GameClient>();

        public Action<byte[]> decrypt = (arr) => { };

        public readonly ServerPacketHandler packetHandler;

        public GameClient(TcpClient tcpClient, ScrambledKeyPair scrambledKeyPair, byte[] blowfishKey)
        {
            packetHandler = new ServerPacketHandler(this);
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
            Random rnd = new Random();

            RsaPair = scrambledKeyPair;
            BlowfishKey = blowfishKey;

            crypt = new EmptyNetworkCrypt();

            Task.Factory.StartNew(ReadAsync);
        }

        public async Task SendAsync(Packet p)
        {
            logger.Information($"Cервер отправялет пакет:{p.FirstOpcode:X2}");
            byte[] data = p.GetBuffer();
            crypt.Encrypt(data);

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

                    byte[] body = new byte[bodyLength];
                    await ReadBodyAsync(body, bodyLength);

                    var packet = new Packet(1, body);

                    _ = Task.Factory.StartNew(() => packetHandler.Handle(packet));
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка: {ex.Message}");
                //TODO: Тут надо новерно закрывать соединение
            }
        }

        public byte[] EnableCrypt()
        {
            byte[] key = BlowFishKeygen.GetRandomKey();
            var _crypt = new GameCrypt();
            _crypt.SetKey(key);
            crypt = _crypt;
            return key;
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

        private async Task ReadBodyAsync(byte[] body, short lenght)
        {
            int bytesRead = await networkStream.ReadAsync(body, 0, lenght);

            if (bytesRead != lenght)
            {
                throw new Exception("Пакет имеет поврежденную структуру");
            }

            try
            {
                crypt.Decrypt(body);
            }
            catch (Exception e)
            {
                throw new Exception($"Не удалось расшифровать пакет для клиента - {tcpClient.Client.RemoteEndPoint}.");
            }
        }
    }
}
