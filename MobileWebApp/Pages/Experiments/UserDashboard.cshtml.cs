using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

[Authorize]
public class UserDashboardPageModel : PageModel
{
    private readonly ILogger<UserDashboardPageModel> logger;
    private readonly AppUserService appUserService;

    public long UserCount { get; set; }
    public byte PageSize { get; set; }

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public UserDashboardPageModel(ILogger<UserDashboardPageModel> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;

        // TODO: Read from user preference?
        this.PageSize = 5;
    }

    public async Task OnGetAsync()
    {
        UserCount = await appUserService.GetUserCountAsync();
    }

}
