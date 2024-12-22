using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

[Authorize]
public class ExperimentsIndexModel : PageModel
{
    private readonly ILogger<ExperimentsIndexModel> logger;

    public ExperimentsIndexModel(ILogger<ExperimentsIndexModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {

    }
}
