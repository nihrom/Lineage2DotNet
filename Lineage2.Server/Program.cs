using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lineage2.Engine;
using Lineage2.Model;
using Lineage2.Model.GeoEngine;
using Lineage2.Model.GeoEngine.PathFinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Tools.NpcTemplateConvertXmlToJson;
using Lineage2.Database;
using Lineage2.Engine.Repositories;
using Lineage2.Database.Repositories;

namespace Lineage2.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("ServerConfig.json", optional: true);
                })
                .ConfigureServices((hostbuilder, services) =>
                {
                    services.AddHostedService<ServerHostingService>();
                    services.Configure<ServerConfig>(hostbuilder.Configuration.GetSection("ServerConfig"));
                    services.AddDbContext<Lineage2DbContext>(options => 
                        options.UseSqlServer(
                            hostbuilder.Configuration["DB:LocalConnectionString"]
                        )
                    );
                })
                .ConfigureContainer<ContainerBuilder>((hostBuilder, builder) =>
                {
                    builder.RegisterType<DbSeed>()
                        .As<IStartable>()
                        .AutoActivate();

                    builder.RegisterType<GameServer>();
                    builder.RegisterType<ConnectionHandler>();
                    builder.Register<ServerConfig>(c => c.Resolve<IOptions<ServerConfig>>().Value);

                    builder.RegisterType<L2PlayersRepository>().As<IL2PlayersRepository>().InstancePerLifetimeScope();
                    builder.RegisterType<SpawnsRepository>().As<ISpawnsRepository>().InstancePerLifetimeScope();

                    //builder.RegisterType<GeoEngine>().SingleInstance().AutoActivate();
                    //builder.RegisterType<GeoPathFinding>().As<PathFinding>().SingleInstance().AutoActivate();
                    builder.RegisterType<WorldLauncher>().SingleInstance();
                })
                .UseSerilog(Log.Logger, false)
                .UseConsoleLifetime();
    }
}
