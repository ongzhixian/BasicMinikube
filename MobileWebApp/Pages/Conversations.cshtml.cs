using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class ConversationsPageModel : PageModel
{
    private readonly ILogger<ConversationsPageModel> _logger;

    public ConversationsPageModel(ILogger<ConversationsPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
