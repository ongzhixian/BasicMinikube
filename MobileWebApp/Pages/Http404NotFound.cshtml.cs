using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class Http404NotFoundPageModel : PageModel
{
    private readonly ILogger<Http404NotFoundPageModel> _logger;

    public Http404NotFoundPageModel(ILogger<Http404NotFoundPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
