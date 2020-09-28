using Lineage2.Network;
using Lineage2.Network.LoginService.OutWriters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace Lineage2.Server.LoginServerService
{
    public class LoginServerConnection
    {
        private static readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;

        protected TcpClient tcpClient;
        protected NetworkStream networkStream;
        protected Timer Ltimer;
        public bool IsConnected;
        private byte[] _buffer;
        public string Version = "rcs #216";
        public int Build = 0;

        public void Start()
        {
            IsConnected = false;
            try
            {
                tcpClient = new TcpClient(_serverConfig.AuthHost, _serverConfig.AuthPort);
                networkStream = tcpClient.GetStream();
            }
            catch (SocketException ex)
            {
                _logger.Error($"Socket Error: '{ex.SocketErrorCode}'. Message: '{ex.Message}' (Error Code: '{ex.NativeErrorCode}')");
                _logger.Warning("Login server is not responding. Retrying");

                if (Ltimer == null)
                {
                    Ltimer = new Timer
                    {
                        Interval = 2000
                    };
                    Ltimer.Elapsed += ltimer_Elapsed;
                }

                if (!Ltimer.Enabled)
                    Ltimer.Enabled = true;

                return;
            }

            if ((Ltimer != null) && Ltimer.Enabled)
                Ltimer.Enabled = false;

            IsConnected = true;

            var serverInfoWriter = new ServerInfoWriter()
            {
                Host = _serverConfig.Host,
                Port = _serverConfig.Port,
                AuthCode = _serverConfig.AuthCode,
                IsGmOnly = _serverConfig.IsGmOnly,
                IsTestServer = _serverConfig.IsTestServer,
                MaxPlayers = _serverConfig.MaxPlayers
            };

            var loginServicePing = new LoginServicePingWriter(Version, Build);

            SendPacket(serverInfoWriter);
            SendPacket(loginServicePing);
            Read();
        }

        private void ltimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Start();
        }

        public void Read()
        {
            try
            {
                _buffer = new byte[2];
                networkStream.BeginRead(_buffer, 0, 2, OnReceiveCallbackStatic, null);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}");
                Termination();
            }
        }

        private void OnReceiveCallbackStatic(IAsyncResult result)
        {
            try
            {
                int rs = networkStream.EndRead(result);
                if (rs <= 0)
                    return;

                short length = BitConverter.ToInt16(_buffer, 0);
                _buffer = new byte[length];
                networkStream.BeginRead(_buffer, 0, length, new AsyncCallback(OnReceiveCallback), result.AsyncState);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}");
                Termination();
            }
        }

        private void OnReceiveCallback(IAsyncResult result)
        {
            networkStream.EndRead(result);

            byte[] buff = new byte[_buffer.Length];
            _buffer.CopyTo(buff, 0);

            Packet packet = new Packet(1, buff);
            _logger.Information($"Received packet with Opcode:{packet.FirstOpcode:X2}");
            //TODO:Стремная херня. Кажется тут не обязательно создавать новый поток
            new System.Threading.Thread(Read).Start();
        }

        private void Termination()
        {
            if (_paused)
                return;

            _logger.Error("Reconnecting...");
            Start();
        }

        public void SendPacket(OutPacketWriter pk)
        {
            pk.Write();

            List<byte> blist = new List<byte>();
            byte[] db = pk.PacketBytes;

            short len = (short)db.Length;
            blist.AddRange(BitConverter.GetBytes(len));
            blist.AddRange(db);

            networkStream.Write(blist.ToArray(), 0, blist.Count);
            networkStream.Flush();
        }

        private bool _paused;

        public void LoginFail(string code)
        {
            _paused = true;
            _logger.Error($"{code}. Please check configuration, server paused.");
            try
            {
                networkStream.Close();
                tcpClient.Close();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}");
            }
        }

    }
}
