using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class ContactsPageModel : PageModel
{
    private readonly ILogger<ContactsPageModel> _logger;

    public ContactsPageModel(ILogger<ContactsPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
