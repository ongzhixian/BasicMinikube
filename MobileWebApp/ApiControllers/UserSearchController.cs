using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Api;

using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class UserSearchController : ControllerBase
{
    private readonly ILogger<UserSearchController> logger;
    private readonly AppUserService appUserService;

    public UserSearchController(ILogger<UserSearchController> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
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
        var results = await appUserService.FindMatchingUserAsync(searchCriteria);
        return results.Select(r => r.Username);
    }

    //// POST api/<UserController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<UserController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<UserController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
