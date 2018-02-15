namespace AzureContainers
{
    using Configurations;
    using Implementations;
    using System.IO;
    using Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;

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
                .AddDebug());

            // build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", false)
                .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<AzureConfig>(configuration.GetSection("Azure"));

            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File($"C:/logs/SAM/RerouteBlobs.log")
                .MinimumLevel.Debug()
                .CreateLogger();
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            // add services
            serviceCollection.AddTransient<IBlobService, BlobService>();
            serviceCollection.AddTransient<IAzureStorage, AzureStorage>();

            // add app
            serviceCollection.AddTransient<App>();
        }
    }
}
