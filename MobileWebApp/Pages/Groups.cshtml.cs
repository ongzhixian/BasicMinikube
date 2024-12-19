using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MobileWebApp.Pages;

public class GroupsPageModel : PageModel
{
    private readonly ILogger<GroupsPageModel> _logger;

    public GroupsPageModel(ILogger<GroupsPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
