using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class ScanQrPageModel : PageModel
{
    private readonly ILogger<ScanQrPageModel> _logger;

    public ScanQrPageModel(ILogger<ScanQrPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
