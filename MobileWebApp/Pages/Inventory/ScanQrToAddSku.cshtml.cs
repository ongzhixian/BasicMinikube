using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class ScanQrToAddSkuPageModel : PageModel
{
    private readonly ILogger<ScanQrToAddSkuPageModel> logger;

    [BindProperty]
    public IFormFile QrCodeImage { get; set; }

    public string ScannedData { get; set; }

    public ScanQrToAddSkuPageModel(ILogger<ScanQrToAddSkuPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {

    }


    public IActionResult OnPostAsync()
    {


        return Page();
    }

}
