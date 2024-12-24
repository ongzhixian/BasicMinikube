using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class ChangePasswordPageModel : PageModel
{
    private readonly ILogger<ChangePasswordPageModel> logger;
    private readonly AppUserService appUserService;
    private readonly AppUserAuthenticationService appUserAuthenticationService;

    [BindProperty, Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    public ChangePasswordPageModel(ILogger<ChangePasswordPageModel> logger, AppUserService appUserService, AppUserAuthenticationService appUserAuthenticationService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
        this.appUserAuthenticationService = appUserAuthenticationService;
    }

    public void OnGet()
    {
    }


    public IActionResult OnPost(string? returnUrl = null)
    {
        if (User.Identity == null)
        {
            ModelState.AddModelError(string.Empty, "User has no identity.");
            return Page();
        }

        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError(string.Empty, "User is not authenticated.");
            return Page();
        }

        var username = User.Identity.Name;

        if (ModelState.IsValid && !string.IsNullOrEmpty(username))
        {
            //var user = await appUserService.GetUserAsync(username);

            //appUserAuthenticationService.UpdatePassword(username, NewPassword);

            //var credentialsAreValid = await appUserAuthenticationService.CredentialsAreValidAsync(Username.ToUpperInvariant(), Password);

            //if (credentialsAreValid)
            //{
            //    var claims = await appUserAuthorizationService.GetClaimsAsync(Username.ToUpperInvariant());

            //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            //    return Redirect(returnUrl);
            //}
        }

        //ModelState.AddModelError(string.Empty, "Invalid login attempt");

        //return new ForbidResult();
        return Page();
    }
}
