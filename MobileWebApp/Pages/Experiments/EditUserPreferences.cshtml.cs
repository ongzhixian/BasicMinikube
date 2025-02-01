using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.RazorPages;
using MobileWebApp.Services;
using MobileWebApp.MongoDbModels;

namespace MobileWebApp.Pages.Experiments;

public class EditUserPreferencesModel : PageModel
{
    private readonly ILogger<EditUserPreferencesModel> logger;
    private readonly AppUserService appUserService;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Preferred Page Size")]
    public int PreferredPageSize { get; set; }

    public EditUserPreferencesModel(ILogger<EditUserPreferencesModel> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
    }

    public async Task OnGetAsync()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            //return new ForbidResult();
            return;
        }

        var appUser = await appUserService.GetUserAsync(User.Identity.Name);

        if (appUser == null) return;

        PreferredPageSize = appUser.UserPreferences.PreferredPageSize;
    }

    public async Task OnPostAsync()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            //return new ForbidResult();
            return;
        }

        var appUser = await appUserService.GetUserAsync(User.Identity.Name);

        if (appUser == null) return;

        appUser.UserPreferences.PreferredPageSize = (byte)PreferredPageSize;

        await appUserService.UpdateUserAsync(appUser);

        HttpContext.Session.SetInt32(UserPreferences.PREFERRED_PAGE_SIZE, appUser.UserPreferences.PreferredPageSize);
    }
}
