using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lineage2.Server
{
    public class GameServer
    {
        private readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;
        private TcpListener _listener;

        public GameServer(ILogger logger, ServerConfig serverConfig)
        {
            _logger = logger;
            _serverConfig = serverConfig;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _serverConfig.Port);

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

            _logger.Information($"Listening Gameservers on port {_serverConfig.Port}");

            WaitForClients();
        }

        private void WaitForClients()
        {
            _listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            TcpClient clientSocket = _listener.EndAcceptTcpClient(asyncResult);

            _logger.Information($"Received connection request from: {clientSocket.Client.RemoteEndPoint}");

            //TODO:Тут надо как обрабатывать подключенного пользователя

            WaitForClients();
        }
    }
}
