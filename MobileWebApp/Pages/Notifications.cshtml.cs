using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class NotificationsPageModel : PageModel
{
    private readonly ILogger<NotificationsPageModel> _logger;

    public NotificationsPageModel(ILogger<NotificationsPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
