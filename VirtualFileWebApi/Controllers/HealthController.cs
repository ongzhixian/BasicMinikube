using Microsoft.AspNetCore.Mvc;

namespace VirtualFileWebApi.Controllers;

[ApiController]
[Route("/")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> logger;

    public HealthController(ILogger<HealthController> logger)
    {
        this.logger = logger;
    }

    [HttpGet(Name = "GetHealth")]
    [Route("/health")]
    public IResult Get()
    {
        return TypedResults.Ok("Healthy");
    }
}
