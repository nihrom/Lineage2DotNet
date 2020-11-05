using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;

namespace Lineage2.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var serverConfig = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(@"ServerConfig.json"));
            var gameServer = new GameServer(Log.Logger, serverConfig);
            gameServer.Start();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
