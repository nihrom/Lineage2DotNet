﻿using L2Crypt;
using Lineage2.Unility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lineage2.Network
{
    //TODO: Реализовать остальные состояния соединения (паузы, отключения и тд)
    public class L2Connection
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private readonly ILogger logger = Log.Logger.ForContext<L2Connection>();

        public INetworkCrypt Crypt { get; set; }

        public event Action<Packet> ReceivedPacket;
        public event Action<Packet> SendingPacket;

        public L2Connection(TcpClient tcpClient, INetworkCrypt crypt)
        {
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
            Crypt = crypt;
        }

        /// <summary>
        /// Отправить пакет по соединению
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task SendAsync(Packet p)
        {
            SendingPacket?.Invoke(p);
            byte[] data = p.GetBuffer();
            Crypt.Encrypt(data);

            byte[] lengthBytes = BitConverter.GetBytes((short)(data.Length + 2));
            byte[] message = new byte[data.Length + 2];

            lengthBytes.CopyTo(message, 0);
            data.CopyTo(message, 2);

            await networkStream.WriteAsync(message, 0, message.Length);
            await networkStream.FlushAsync();
        }

        /// <summary>
        /// Цикл чтения пакетов из соединения
        /// </summary>
        /// <returns></returns>
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

                    ReceivedPacket?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка при чтении пакета: {ex.Message}");
                //TODO: Тут надо новерно закрывать соединение
            }
        }

        /// <summary>
        /// Получает длину пакета из первых двух байт
        /// </summary>
        /// <returns></returns>
        private async Task<short> ReadBodyLengthAsync()
        {
            byte[] buffer = new byte[2];
            int bytesRead = await networkStream.ReadAsync(buffer, 0, 2);

            if (bytesRead != 2)
                throw new Exception("Пакет имеет поврежденную структуру");

            short length = BitConverter.ToInt16(buffer, 0);

            return (short)(length - 2);
        }

        /// <summary>
        /// Заполняет массив телом пакета
        /// </summary>
        /// <param name="body">Массив для заполнения</param>
        /// <param name="lenght">Длина пакета</param>
        /// <returns></returns>
        private async Task ReadBodyAsync(byte[] body, short lenght)
        {
            int bytesRead = await networkStream.ReadAsync(body, 0, lenght);

            if (bytesRead != lenght)
                throw new Exception("Пакет имеет поврежденную структуру");

            //TODO: Возможно декрипт надо обернуть в try catch
            Crypt.Decrypt(body);
        }

    }
}
