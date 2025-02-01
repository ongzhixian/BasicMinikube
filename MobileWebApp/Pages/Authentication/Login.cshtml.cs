using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.MongoDbModels;
using MobileWebApp.Services;

namespace MobileWebApp.Pages.Authentication;

[AllowAnonymous]
public class LoginPageModel : PageModel
{
    private readonly ILogger<LoginPageModel> logger;

    private readonly AppUserAuthenticationService appUserAuthenticationService;
    private readonly AppUserAuthorizationService appUserAuthorizationService;
    private readonly AppUserService appUserService;
    private readonly SessionService sessionService;


    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public LoginPageModel(ILogger<LoginPageModel> logger, 
        AppUserAuthenticationService appUserAuthenticationService, 
        AppUserAuthorizationService appUserAuthorizationService,
        AppUserService appUserService,
        SessionService sessionService)
    {
        this.logger = logger;
        this.appUserAuthenticationService = appUserAuthenticationService;
        this.appUserAuthorizationService = appUserAuthorizationService;
        this.appUserService = appUserService;
        this.sessionService = sessionService;
    }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        ReturnUrl = returnUrl;

    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var credentialsAreValid = await appUserAuthenticationService.CredentialsAreValidAsync(Username.ToUpperInvariant(), Password);

            if (credentialsAreValid)
            {
                var claims = await appUserAuthorizationService.GetClaimsAsync(Username.ToUpperInvariant());

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await sessionService.LoadUserPreferencesToSessionAsync(Username.ToUpperInvariant());
                //HttpContext.Session.SetInt32(UserPreferences.PREFERRED_PAGE_SIZE, appUser.UserPreferences.PreferredPageSize);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                
                return Redirect(returnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt");

        return new ForbidResult();
        //return Page();
    }

}
