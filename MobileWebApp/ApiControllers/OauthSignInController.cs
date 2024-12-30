using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Api;

using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.ApiControllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class OauthSignInController : ControllerBase
{
    private readonly ILogger<OauthSignInController> logger;
    private readonly AppRoleService appRoleService;
    private readonly AppUserService appUserService;
    private readonly AppUserAuthorizationService appUserAuthorizationService;

    public OauthSignInController(ILogger<OauthSignInController> logger
        , AppRoleService appRoleService
        , AppUserService appUserService
        , AppUserAuthorizationService appUserAuthorizationService)
    {
        this.logger = logger;
        this.appRoleService = appRoleService;
        this.appUserService = appUserService;
        this.appUserAuthorizationService = appUserAuthorizationService;
    }

    // api/oauth-sign-in/

    // GET: api/<UserController>
    [HttpGet("google")]
    public async Task<LocalRedirectResult> GoogleSignInAsync(string? returnUrl = null, string? remoteError = null)
    {
        // Get the information about the user from the external login provider
        var GoogleUser = this.User.Identities.FirstOrDefault();

        if (GoogleUser != null && GoogleUser.IsAuthenticated)
        {
            // Create AppUser if not exists
            Claim? emailClaim = GoogleUser.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Email);
            Claim? nameIdentifierClaim = GoogleUser.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier);
            string username = $"{GoogleUser.AuthenticationType}:{nameIdentifierClaim?.Value}".ToUpperInvariant();
            
            var appUser = await appUserService.GetUserAsync(username);

            if (appUser == null)
            {
                appUser = new MongoDbModels.AppUser();
                appUser.OAuthProvider = GoogleUser.AuthenticationType;
                appUser.Username = username;
                appUser.DisplayName = GoogleUser.Name;
                appUser.Claims = [
                    new Claim(ClaimTypes.Name, username)
                ];

                await appUserService.AddAppUserAsync(appUser);
            }

            var claims = await appUserAuthorizationService.GetClaimsAsync(username);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            //var authProperties = new AuthenticationProperties { IsPersistent = true };
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(GoogleUser), authProperties);

            return LocalRedirect(returnUrl ?? "/");
        }

        // If the user is not authenticated, redirect to the home page
        return LocalRedirect("/");
    }

    [HttpPost("/signin-googlex")]
    public IEnumerable<string> Post()
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
