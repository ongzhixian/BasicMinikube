using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class NativeScanQrPageModel : PageModel
{
    private readonly ILogger<NativeScanQrPageModel> logger;

    [BindProperty]
    public IFormFile QrCodeImage { get; set; }

    public string ScannedData { get; set; }

    public NativeScanQrPageModel(ILogger<NativeScanQrPageModel> logger)
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
