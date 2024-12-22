using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

[AllowAnonymous]
public class RegisterNewAppUserPageModel : PageModel
{
    private readonly ILogger<RegisterNewAppUserPageModel> logger;

    private readonly AppUserAuthenticationService appUserAuthenticationService;
    private readonly AppUserAuthorizationService appUserAuthorizationService;
    private readonly AppUserService appUserService;
            
    private readonly EmailService emailService;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public RegisterNewAppUserPageModel(ILogger<RegisterNewAppUserPageModel> logger, 
        AppUserAuthenticationService appUserAuthenticationService, 
        AppUserAuthorizationService appUserAuthorizationService,
        AppUserService appUserService,
        EmailService emailService)
    {
        this.logger = logger;
        this.appUserAuthenticationService = appUserAuthenticationService;
        this.appUserAuthorizationService = appUserAuthorizationService;
        this.appUserService = appUserService;
        this.emailService = emailService;
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

        //EmailMessageModel testEmail = new EmailMessageModel
        //{
        //    Sender = new Email
        //    {
        //        Address = "MS_1puDAp@trial-z86org80oq04ew13.mlsender.net",
        //        Name = "zhixian"
        //    },
        //    Recipients = new Email[]
        //    {
        //            new() { Address = "zhixian@hotmail.com", Name = "zhixian" }
        //    },
        //    Subject = "Test email message"
        //};
        //testEmail.Html = "&nbsp;";

        //await emailService.SendEmailAsync(testEmail);

        if (ModelState.IsValid)
        {
            //await appUserService.AddAppUserAsync(Username, Password);

            //if (credentialsAreValid)
            //{
            //    var claims = appUserAuthorizationService.GetClaims(Username);

            //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            //    return Redirect(returnUrl);
            //}

            await appUserService.AddAppUserAsync(Username, Password);

            ViewData["message"] = "User registered.";
            ViewData["message-class"] = "positive";

            return Page();
        }

        ViewData["message-class"] = "negative";
        ModelState.AddModelError(string.Empty, "Registration failed.");

        return Page();
    }

}
