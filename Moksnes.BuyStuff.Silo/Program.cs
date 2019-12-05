using System;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moksnes.BuyStuff.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace ConsoleApp1
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                IConfigurationBuilder builder = new ConfigurationBuilder();
                var configuration = builder.AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true)
                    .AddUserSecrets(typeof(Program).Assembly)
                    .Build();

                var host = await StartSilo("cluster1", "service1", 1000, 1002, configuration);
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo(string clusterId, string serviceId, int siloPort, int gatewayPort, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");

            var builder = new SiloHostBuilder()
                .Configure<ProcessExitHandlingOptions>(options => { options.FastKillOnProcessExit = false; })
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = clusterId;
                    options.ServiceId = serviceId;
                })
                .AddAzureBlobGrainStorage("azureBlob", options =>
                {
                    options.UseJson = true;
                    options.ConnectionString = connectionString;
                    
                })
                .UseAzureStorageClustering(options => options.ConnectionString = connectionString)
                .AddLogStorageBasedLogConsistencyProvider()
                //.ConfigureEndpoints(Dns.GetHostName(), siloPort, gatewayPort)
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));

            var host = builder.Build();
            await host.StartAsync().ConfigureAwait(false);

            return host;
        }

        //private static async Task<ISiloHost> StartSilo()
        //{
        //    const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=OrleansDb;Trusted_Connection=True;MultipleActiveResultSets=True";

        //    // define the cluster configuration
        //    var builder = new SiloHostBuilder()
        //        .UseLocalhostClustering()
        //        .Configure<ClusterOptions>(options =>
        //        {
        //            options.ClusterId = "dev";
        //            options.ServiceId = "OrleansBasics";
        //        })
        //        .AddAdoNetGrainStorage("OrleansStorage", options =>
        //        {
        //            options.Invariant = "System.Data.SqlClient";
        //            options.ConnectionString = connectionString;
        //            options.UseJsonFormat = true;
        //        })
        //        .AddLogStorageBasedLogConsistencyProvider()
        //        //.UseAdoNetClustering(options =>
        //        //{
        //        //    options.ConnectionString = connectionString;
        //        //    options.Invariant = "System.Data.SqlClient";
        //        //})
        //        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
        //        .ConfigureLogging(logging => logging.AddConsole());

        //    var host = builder.Build();
        //    await host.StartAsync();
        //    return host;
        //}
    }
}
