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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await gameServer.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.Information("Server Stop");

            await gameServer.Stop();
        }
    }
}
