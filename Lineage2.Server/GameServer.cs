using L2Crypt;
using Lineage2.Engine;
using Lineage2.Network;
using Lineage2.Unility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class GameServer
    {
        private readonly ILogger logger;
        private readonly ServerConfig _serverConfig;
        private TcpListener _listener;
        private readonly WorldLauncher worldLauncher;

        ScrambledKeyPair scrambledKeyPair = new ScrambledKeyPair(ScrambledKeyPair.genKeyPair());
        private byte[] blowfishKey = new byte[16];

        public GameServer(ILogger logger, ServerConfig serverConfig, WorldLauncher worldLauncher)
        {
            this.logger = logger;
            _serverConfig = serverConfig;
            this.worldLauncher = worldLauncher;

            new Random().NextBytes(blowfishKey);
        }

        public async Task Start()
        {
            await worldLauncher.Launche();

            _listener = new TcpListener(IPAddress.Parse(_serverConfig.Host), _serverConfig.Port);

            try
            {
                _listener.Start();
            }
            catch (SocketException ex)
            {
                logger.Error($"Ошибка в сокете: '{ex.SocketErrorCode}'. Сообщение: '{ex.Message}' (Код ошибки: '{ex.NativeErrorCode}')");
                logger.Information("Нажмите ENTER для завершения...");
                Console.Read();
                Environment.Exit(0);
            }

            logger.Information($"Сервер аутентификации слушает входящих клиентов на {_listener.LocalEndpoint}");

            WaitForClients();
        }

        public async Task Stop()
        {
            //TODO: Надо добавить по цепочке методы для штатной оставноки сервера. Отключать пользователей, сохранять данные.
        }

        private async void WaitForClients()
        {
            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = Task.Factory.StartNew(() => AcceptClient(client));
            }
        }

        private void AcceptClient(TcpClient client)
        {
            logger.Information($"Получен запрос на подключение от: {client.Client.RemoteEndPoint}");

            byte[] key = BlowFishKeygen.GetRandomKey();
            var crypt = new GameCrypt(key);
            var connection = new L2Connection(client, crypt);
            var loginClient = new GameClient(connection, worldLauncher);

            Task.Factory.StartNew(connection.ReadAsync);
        }
    }
}
