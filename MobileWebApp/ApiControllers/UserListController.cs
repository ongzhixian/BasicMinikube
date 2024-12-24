using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserListController : ControllerBase
    {
        private readonly ILogger<UserListController> logger;
        private readonly AppUserService appUserService;

        public UserListController(ILogger<UserListController> logger, AppUserService appUserService)
        {
            this.logger = logger;
            this.appUserService = appUserService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            return await GetAsync(1);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> GetAsync(int id, byte pageSize = 5)
        {
            // If we say that user preference should overwrite query parameters, we overwrite it here.

            var pageNumber = id;
            var results = await appUserService.GetUserListAsync(pageNumber, pageSize);
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
}
