using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class ScanQrPageModel : PageModel
{
    private readonly ILogger<ScanQrPageModel> _logger;

    [BindProperty]
    public IFormFile QrCodeImage { get; set; }

    public string ScannedData { get; set; }

    public ScanQrPageModel(ILogger<ScanQrPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }


    public IActionResult OnPostAsync()
    {


        return Page();
    }

}
