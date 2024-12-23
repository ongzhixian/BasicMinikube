using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

[Authorize]
public class UserDashboardPageModel : PageModel
{
    private readonly ILogger<UserDashboardPageModel> logger;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public UserDashboardPageModel(ILogger<UserDashboardPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {

    }
}
