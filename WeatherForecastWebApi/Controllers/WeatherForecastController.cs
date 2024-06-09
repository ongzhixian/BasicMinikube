using Microsoft.AspNetCore.Mvc;

using WareLogix.WebApi.Metrics;

namespace WeatherForecastWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> logger;
    private readonly RequestMetric requestMetric;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, RequestMetric requestMetric)
    {
        this.logger = logger;
        this.requestMetric = requestMetric;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {

        logger.LogInformation("Weather forecast requested");

        requestMetric.Increment();

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
