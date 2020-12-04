using Lineage2.Model;
using Lineage2.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


             var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения

            //string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<Lineage2DbContext>();
            var options = optionsBuilder
                .UseSqlServer(config["DB:LocalConnectionString"])
                .Options;

            using (Lineage2DbContext db = new Lineage2DbContext(options))
            {

            }

            new NpcFactory().Initialize();
            var serverConfig = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText(@"ServerConfig.json"));
            var connectionHandller = new ConnectionHandler();
            var gameServer = new GameServer(Log.Logger, serverConfig, connectionHandller);
            gameServer.Start();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
