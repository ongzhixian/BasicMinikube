using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

[AllowAnonymous]
public class AccessDeniedPageModel : PageModel
{
    private readonly ILogger<AccessDeniedPageModel> _logger;

    public AccessDeniedPageModel(ILogger<AccessDeniedPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
