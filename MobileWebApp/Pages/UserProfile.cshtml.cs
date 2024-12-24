using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class UserProfilePageModel : PageModel
{
    private readonly ILogger<UserProfilePageModel> logger;
    private readonly AppUserService appUserService;

    public string Username { get; set; } = string.Empty;
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
    //public string DisplayName { get; set; }

    public string[] ClaimNames { get; set; } = [];

    public DateTime PasswordLastUpdateDateTime { get; set; }

    public UserProfilePageModel(ILogger<UserProfilePageModel> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
    }

    public async Task OnGetAsync()
    {
        if (User.Identity == null || (User.Identity.IsAuthenticated == false)) return;

        var user = await appUserService.GetUserAsync(User.Identity?.Name ?? string.Empty);

        this.Username = user.Username;
        //this.FirstName = user.Username;
        //this.LastName = user.Username;
        //this.DisplayName = user.Username;

        PasswordLastUpdateDateTime = user.PasswordLastUpdateDateTime;

        ClaimNames = user.Claims.Where(r => r.Type == System.Security.Claims.ClaimTypes.Role).Select(r => r.Value).ToArray();
    }
}
