using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.RazorPages;

using QRCoder;

namespace MobileWebApp.Pages;

public class ApplicationListPageModel : PageModel
{
    private readonly ILogger<ApplicationListPageModel> logger;

    public ApplicationListPageModel(ILogger<ApplicationListPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
    }

}
