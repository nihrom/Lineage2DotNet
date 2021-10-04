using Lineage2.Engine;
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
        private ConnectionHandler connectionHandler;
        private readonly WorldLauncher worldLauncher;

        public GameServer(ILogger logger, ServerConfig serverConfig, ConnectionHandler connectionHandler, WorldLauncher worldLauncher)
        {
            this.logger = logger;
            _serverConfig = serverConfig;
            this.connectionHandler = connectionHandler;
            this.worldLauncher = worldLauncher;
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

            connectionHandler.Handle(client);
        }
    }
}
