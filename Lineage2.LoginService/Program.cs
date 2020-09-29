using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;

namespace Lineage2.LoginService
{
    class Program
    {
        static void  Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var loginServiceConfig = JsonConvert.DeserializeObject<LoginServiceConfig>(File.ReadAllText(@"LoginServiceConfig.json"));
            var loginServer = new LoginServer(Log.Logger, loginServiceConfig);
            loginServer.Start();

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
