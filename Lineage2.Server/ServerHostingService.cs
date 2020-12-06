using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lineage2.Server
{
    public class ServerHostingService : IHostedService
    {
        private readonly ILogger logger;
        private readonly GameServer gameServer;

        public ServerHostingService(ILogger logger, GameServer gameServer)
        {
            this.logger = logger;
            this.gameServer = gameServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            gameServer.Start();
            return Task.Delay(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //TODO: Надо добавить по цепочке методы для штатной оставноки сервера. Отключать пользователей, сохранять данные.
            logger.Information("Server Stop");
            return Task.Delay(0);
        }
    }
}
