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
        private readonly ILogger _logger;
        private readonly LoginServiceConfig _config;
        private TcpListener _listener;

        public LoginServer(ILogger logger, LoginServiceConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Parse(_config.Host), _config.LoginPort);

            try
            {
                _listener.Start();
            }
            catch (SocketException ex)
            {
                _logger.Error($"Socket Error: '{ex.SocketErrorCode}'. Message: '{ex.Message}' (Error Code: '{ex.NativeErrorCode}')");
                _logger.Information("Press ENTER to exit...");
                Console.Read();
                Environment.Exit(0);
            }
            _logger.Information($"Auth server listening clients at {_config.Host}:{_config.LoginPort}");
            WaitForClients();
        }

        private async void WaitForClients()
        {
            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
#pragma warning disable 4014
                Task.Factory.StartNew(() => AcceptClient(client));
#pragma warning restore 4014
            }
        }

        private void AcceptClient(TcpClient client)
        {
            _logger.Information($"Received connection request from: {client.Client.RemoteEndPoint}");

            //TODO:Тут надо обрабатывать новые подключения клментов
        }
    }
}
