using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.MongoDbModels;

namespace MobileWebApp.Pages;

[AllowAnonymous]
public class ExternalSignInPageModel : PageModel
{
    private readonly ILogger<ExternalSignInPageModel> logger;

    public ExternalSignInPageModel(ILogger<ExternalSignInPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {

    }


    public ChallengeResult OnGetGoogle()
    {

        var x = Url.Page("./GoogleLoginCallbackXXX", pageHandler: "Callback");

        var y = Url.Action(action: "GoogleSignIn", controller: "OauthSignIn");

        //string redirectUrl = Url.Action("GoogleResponse", "Account");
        var authenticationProperties = new AuthenticationProperties
        {
            //RedirectUri = Url.Page("./GoogleLoginCallbackXXX", pageHandler: "Callback"),
            RedirectUri = y
        };

        return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        
    }
}
