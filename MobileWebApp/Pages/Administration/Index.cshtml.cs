using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

[Authorize]
public class ManagementIndexModel : PageModel
{
    private readonly ILogger<ManagementIndexModel> _logger;

    public ManagementIndexModel(ILogger<ManagementIndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
