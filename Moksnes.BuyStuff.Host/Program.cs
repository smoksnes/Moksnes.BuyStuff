using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moksnes.BuyStuff.Api;
using Moksnes.BuyStuff.Silo;

namespace Moksnes.BuyStuff.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await Run(args);
        }

        public static async Task Run(string[] args)
        {
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile($"appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true)
                        .AddUserSecrets(typeof(Program).Assembly);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans((context, siloBuilder) =>
                {
                    //siloBuilder.UseLocalhostClustering();
                    // Configure Orleans
                    siloBuilder.ConfigureSilo(context);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    /* Configure cross-cutting concerns such as logging */
                })
                .ConfigureServices(services =>
                {
                    /* Configure shared services */
                    //IClusterClient client = new ClientBuilder()
                    //    // Clustering information
                    //    .UseLocalhostClustering()
                    //    //.Configure<ClusterOptions>(options =>
                    //    //{
                    //    //    options.ClusterId = "cluster1";
                    //    //    options.ServiceId = "service1";
                    //    //})
                    //    // Clustering provider
                    //    // Application parts: just reference one of the grain interfaces that we use
                    //    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPost).Assembly))
                    //    .Build();
                    //client.Connect().Wait();

                    //services.AddSingleton(client);
                })
                .UseConsoleLifetime()
                .Build();

            // Start the host and wait for it to stop.
            await host.RunAsync();
        }


    }
}
