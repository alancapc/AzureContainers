using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RerouteBlobs.Configurations;
using RerouteBlobs.Implementations;
using RerouteBlobs.Interfaces;
using Serilog;
using Serilog.Sinks.File;

namespace RerouteBlobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // entry to run app
            serviceProvider.GetService<App>().Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add configured instance of logging
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddSerilog()
                .AddDebug());

            // add logging
            serviceCollection.AddLogging();

            // build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", false)
                .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<AzureConfig>(configuration.GetSection("Azure"));

            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File($"C:/logs/RerouteBlobs.log")
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

            // add services
            serviceCollection.AddTransient<IBlobService, BlobService>();
            serviceCollection.AddTransient<IAzureStorage, AzureStorage>();

            // add app
            serviceCollection.AddTransient<App>();
        }
    }
}
