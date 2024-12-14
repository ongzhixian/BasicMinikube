using Microsoft.AspNetCore.Mvc;

namespace ObjectStoreWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> logger;

    public HealthController(ILogger<HealthController> logger)
    {
        this.logger = logger;
    }

    [HttpGet("/health", Name = "GetHealth")]
    public IResult Get()
    {
        return TypedResults.Ok("Healthy");
    }
}
