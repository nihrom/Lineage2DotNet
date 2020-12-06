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
    public class Server : IHostedService
    {
        TestClass testClass;
        ILogger logger;
        public Server(TestClass testClass, ILogger logger)
        {
            this.testClass = testClass;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.Information($"Server Start - {testClass.GetInt()}");
            return Task.Delay(3000);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.Information("Server Stop");
            return Task.Delay(1000);
        }
    }
}
