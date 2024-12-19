using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class UserProfilePageModel : PageModel
{
    private readonly ILogger<UserProfilePageModel> _logger;

    public UserProfilePageModel(ILogger<UserProfilePageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
