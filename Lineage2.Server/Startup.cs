using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Server
{
    public class Startup
    {
        //    public Startup(IHostingEnvironment env)
        //    {
        //        var builder = new ConfigurationBuilder()
        //            .SetBasePath(env.ContentRootPath)
        //            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        //            .AddEnvironmentVariables();

        //        this.Configuration = builder.Build();
        //    }

        //    public IConfigurationRoot Configuration { get; private set; }

        //    public ILifetimeScope AutofacContainer { get; private set; }

        //    // ConfigureServices is where you register dependencies. This gets
        //    // called by the runtime before the ConfigureContainer method, below.
        //    public void ConfigureServices(IServiceCollection services)
        //    {
        //        // Add services to the collection. Don't build or return
        //        // any IServiceProvider or the ConfigureContainer method
        //        // won't get called. Don't create a ContainerBuilder
        //        // for Autofac here, and don't call builder.Populate() - that
        //        // happens in the AutofacServiceProviderFactory for you.
        //        services.AddOptions();
        //    }

        //    // ConfigureContainer is where you can register things directly
        //    // with Autofac. This runs after ConfigureServices so the things
        //    // here will override registrations made in ConfigureServices.
        //    // Don't build the container; that gets done for you by the factory.
        //    public void ConfigureContainer(ContainerBuilder builder)
        //    {
        //        // Register your own things directly with Autofac here. Don't
        //        // call builder.Populate(), that happens in AutofacServiceProviderFactory
        //        // for you.
        //        //builder.RegisterModule(new MyApplicationModule());
        //    }

        //    // Configure is where you add middleware. This is called after
        //    // ConfigureContainer. You can use IApplicationBuilder.ApplicationServices
        //    // here if you need to resolve things from the container.
        //    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        //    {
        //        // If, for some reason, you need a reference to the built container, you
        //        // can use the convenience extension method GetAutofacRoot.
        //        this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

        //        loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
        //        loggerFactory.AddDebug();
        //        app.UseMvc();
        //    }
    }
}
