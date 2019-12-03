using System;
using System.Threading.Tasks;
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
                var host = await StartSilo();
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

        private static async Task<ISiloHost> StartSilo()
        {
            const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=OrleansDb;Trusted_Connection=True;MultipleActiveResultSets=True";

            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .AddAdoNetGrainStorage("OrleansStorage", options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = connectionString;
                    options.UseJsonFormat = true;
                })
                .AddLogStorageBasedLogConsistencyProvider()
                //.UseAdoNetClustering(options =>
                //{
                //    options.ConnectionString = connectionString;
                //    options.Invariant = "System.Data.SqlClient";
                //})
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
