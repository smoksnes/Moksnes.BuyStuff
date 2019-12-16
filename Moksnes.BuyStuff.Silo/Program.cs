using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moksnes.BuyStuff.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Moksnes.BuyStuff.Silo
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                IConfigurationBuilder builder = new ConfigurationBuilder();
                var configuration = builder.AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true)
                    .AddUserSecrets(typeof(Program).Assembly)
                    .Build();

                var host = Host.CreateDefaultBuilder(args)
                    .UseOrleans((context, builder) =>
                    {
                        // Configure Orleans
                        builder.ConfigureSilo(context);
                    })
                    .ConfigureLogging(logging =>
                    {
                        /* Configure cross-cutting concerns such as logging */
                    })
                    .ConfigureServices(services =>
                    {
                        /* Configure shared services */
                    })
                    .UseConsoleLifetime()
                    .Build();

                // Start the host and wait for it to stop.
                await host.RunAsync();


                //Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                //Console.ReadLine();

                //ISiloHostBuilder siloBuilder = new SiloHostBuilder();
                //siloBuilder.ConfigureSilo(configuration);


                //await StartSilo(configuration, default);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        //public static async Task<ISiloHost> StartSilo(IConfiguration configuration, CancellationToken cancellationToken = default)
        //{
        //    //var host = await StartSilo("cluster1", "service1", 1000, 1002, configuration);


        //    var host = builder.Build();
        //    await host.StartAsync(cancellationToken);

        //    return host;
        //}

        //private static async Task<ISiloHost> StartLocalSilo(IConfiguration configuration)
        //{

        //    const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=OrleansDb;Trusted_Connection=True;MultipleActiveResultSets=True";

        //    // define the cluster configuration
        //    var builder = new SiloHostBuilder()
        //        .Configure<ProcessExitHandlingOptions>(options => { options.FastKillOnProcessExit = false; })
        //        .UseLocalhostClustering()
        //        .Configure<ClusterOptions>(options =>
        //        {
        //            options.ClusterId = "cluster1";
        //            options.ServiceId = "service1";
        //        })
        //        .AddAdoNetGrainStorage("OrleansStorage", options =>
        //        {
        //            options.Invariant = "System.Data.SqlClient";
        //            options.ConnectionString = connectionString;
        //            options.UseJsonFormat = true;
        //        })
        //        .AddLogStorageBasedLogConsistencyProvider()
        //        //.ConfigureEndpoints(Dns.GetHostName(), siloPort, gatewayPort)
        //        .ConfigureApplicationParts(parts =>
        //            parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
        //        .ConfigureLogging(logging => logging.AddConsole())
        //        .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));

        //    var host = builder.Build();
        //    await host.StartAsync();
        //    return host;
        //}
    }

    public static class SiloBuilderExtensions
    {
        public static void ConfigureSilo(this ISiloBuilder builder, Microsoft.Extensions.Hosting.HostBuilderContext context)
        {
            builder.ConfigureSilo(context.Configuration);
        }

        public static void ConfigureSilo(this ISiloBuilder builder, IConfiguration configuration)
        {
            var config = new SiloOptions();
            configuration.Bind("SiloOptions", config);

            builder
                .Configure<ProcessExitHandlingOptions>(options => { options.FastKillOnProcessExit = false; })
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster1";
                    options.ServiceId = "service1";
                })
                //.UseAzureStorageClustering(options => options.ConnectionString = connectionString)
                .AddLogStorageBasedLogConsistencyProvider()
                //.ConfigureEndpoints(Dns.GetHostName(), siloPort, gatewayPort)
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));

            if (config.UseAzureStorage)
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                builder.AddAzureBlobGrainStorage("StorageProvider", options =>
                {
                    options.UseJson = true;
                    options.ConnectionString = connectionString;

                });
            }
            else
            {
                var connectionString = configuration.GetConnectionString("AdoStorage");
                builder.AddAdoNetGrainStorage("StorageProvider", options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = connectionString;
                    options.UseJsonFormat = true;
                });
                //.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);

            }
        }
    }
}
