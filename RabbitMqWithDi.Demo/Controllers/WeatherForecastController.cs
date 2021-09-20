using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMqWithDi.Demo.Controllers
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
        private readonly IQueueService _queueService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IQueueService queueService
            )
        {
            _logger = logger;
            _queueService = queueService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("send")]
        public async Task<IActionResult> SendMessageAsync([FromQuery] string message)
        {
            var messageObject = new
            {
                Id = 1,
                Name = "RandomName",
                Message = message
            };

            await _queueService.SendAsync(
                @object: messageObject,
                exchangeName: "exchange.name",
                routingKey: "routing.key");

            return Ok();
        }
    }
}
