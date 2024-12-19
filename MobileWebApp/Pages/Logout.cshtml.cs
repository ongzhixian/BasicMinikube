using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class LogoutPageModel : PageModel
{
    private readonly ILogger<LogoutPageModel> _logger;

    public LogoutPageModel(ILogger<LogoutPageModel> logger)
    {
        _logger = logger;
    }

    public async Task<RedirectToPageResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync();

        return RedirectToPage("Login");
    }
}
