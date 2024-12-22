using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages.Authentication;

public class LogoutPageModel : PageModel
{
    private readonly ILogger<LogoutPageModel> logger;

    public LogoutPageModel(ILogger<LogoutPageModel> logger)
    {
        this.logger = logger;
    }

    public async Task<RedirectToPageResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync();

        return RedirectToPage("Login");
    }
}
