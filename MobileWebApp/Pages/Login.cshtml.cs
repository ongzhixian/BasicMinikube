using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

[AllowAnonymous]
public class LoginPageModel : PageModel
{
    private readonly ILogger<LoginPageModel> _logger;

    [TempData]
    public string ErrorMessage { get; set; }

    [BindProperty, Required]
    public string Username {  get; set; }

    [BindProperty, Required]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    public LoginPageModel(ILogger<LoginPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        ReturnUrl = returnUrl;

    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var verificationResult = true; // TODO

            if (verificationResult)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return Redirect(returnUrl);
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt");

        return Page();
    }
    
}
