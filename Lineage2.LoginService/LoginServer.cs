using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.LoginService
{
    public class LoginServer
    {
        private readonly ILogger logger;
        private readonly LoginServiceConfig config;
        private readonly ConnectionHandler connectionHandler;
        private TcpListener _listener;

        public LoginServer(ILogger logger, LoginServiceConfig config, ConnectionHandler connectionHandler)
        {
            this.logger = logger;
            this.config = config;
            this.connectionHandler = connectionHandler;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Parse(config.Host), config.LoginPort);

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

            logger.Information($"Сервер аутентификации слушает входящих клиентов на {config.Host}:{config.LoginPort}");
            WaitForClients();
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
