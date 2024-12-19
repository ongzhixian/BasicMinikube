using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class SettingsPageModel : PageModel
{
    private readonly ILogger<SettingsPageModel> _logger;

    public SettingsPageModel(ILogger<SettingsPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
