using Microsoft.AspNetCore.Mvc;

namespace WeatherForecastWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly bool useExample;
    private readonly string exampleSetting1Value;
    private readonly string wareLogixConnectionString;
    private readonly string adventureWorksConnectionString;

    public HealthController(IConfiguration configuration)
    {
        useExample = bool.TryParse(configuration["Features:UseExample"], out bool parsedUsedExample) && parsedUsedExample;
        exampleSetting1Value = configuration["Example:Setting1"] ?? "UNDEFINED";
        wareLogixConnectionString = configuration["ConnectionStrings:WareLogix"] ?? "UNDEFINED";
        adventureWorksConnectionString = configuration["ConnectionStrings:AdventureWorks"] ?? "UNDEFINED";
    }

    [HttpGet]
    [Route("/liveness", Name = "GetLiveness")]
    public IActionResult GetLiveness()
    {
        return Ok("Alive");
    }

    [HttpGet]
    [Route("/ready", Name = "GetReady")]
    public IActionResult GetReady()
    {
        return Ok("Ready");
    }

    [HttpGet]
    [Route("/health", Name = "GetHealth")]
    public IActionResult GetHealth()
    {
        return Ok("Healthy");
    }


    [HttpGet]
    [Route("/test", Name = "GetTest")]
    public IActionResult GetTest()
    {
        string path = System.IO.Path.Combine("/opt/app/data", $"dummyFile-{DateTime.Now:yyyyMMddHHmmss}.txt");
        System.IO.File.WriteAllText(path, $"Some dummy content {Guid.NewGuid().ToString()}");

        return Ok($"UseExample {useExample} {exampleSetting1Value} {wareLogixConnectionString} {adventureWorksConnectionString}");
    }
}
