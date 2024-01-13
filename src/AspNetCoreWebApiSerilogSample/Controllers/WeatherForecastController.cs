using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApiSerilogSample.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogTrace("A trace log message");
            _logger.LogDebug("A debug log message");
            _logger.LogInformation("An information log message");
            _logger.LogWarning("A warn log message");
            _logger.LogError("An error log message");
            _logger.LogCritical("A critical log message");
            _logger.LogError(new ArgumentException(), "An error log message with exception");
            _logger.LogCritical(new StackOverflowException(), "A critical log message");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
