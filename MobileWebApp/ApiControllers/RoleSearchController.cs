using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Api;

using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class RoleSearchController : ControllerBase
{
    private readonly ILogger<RoleSearchController> logger;
    private readonly AppRoleService appRoleService;

    public RoleSearchController(ILogger<RoleSearchController> logger, AppRoleService appRoleService)
    {
        this.logger = logger;
        this.appRoleService = appRoleService;
    }

    // GET: api/<UserController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public async Task<IEnumerable<string>> GetAsync(string id)
    {
        var searchCriteria = id;
        var results = await appRoleService.FindMatchingUserAsync(searchCriteria);
        return results.Select(r => r.RoleName);
    }
}
