using System;
using Microsoft.Extensions.Hosting;

namespace Moksnes.BuyStuff.Api.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Moksnes.BuyStuff.Api.Program.CreateHostBuilder(args).Build().Run();
        }
    }
}
