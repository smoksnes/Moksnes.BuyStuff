using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moksnes.BuyStuff.GrainInterfaces;
using Orleans;

namespace Moksnes.BuyStuff.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IClusterClient _client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            try
            {
                await _client.Connect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var post = _client.GetGrain<IPost>(Guid.Parse("75C47CAC-9217-469E-B0BC-176752314079"));
            await post.AddTitle(Guid.NewGuid().ToString());

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
