using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

[Authorize]
public class ManageUserRolesPageModel : PageModel
{
    private readonly ILogger<ManageUserRolesPageModel> logger;
    private readonly AppRoleService appRoleService;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Role Name")]
    public string NewRoleName { get; set; } = string.Empty;

    public IEnumerable<string> AvailableRoles { get; set; } = Enumerable.Empty<string>();
    public long AvailableRolesCount { get; set; }

    public ManageUserRolesPageModel(ILogger<ManageUserRolesPageModel> logger, AppRoleService appRoleService)
    {
        this.logger = logger;
        this.appRoleService = appRoleService;
    }

    public async Task OnGetAsync()
    {
        var getAllRolesTask = appRoleService.GetAllRolesAsync();
        var getAllRolesCountTask = appRoleService.GetAllRolesCountAsync();

        await Task.WhenAll(getAllRolesTask, getAllRolesCountTask);

        AvailableRoles = getAllRolesTask.Result;
        AvailableRolesCount = getAllRolesCountTask.Result;
    }

    public async Task<IActionResult> OnPostAddNewRole()
    {
        try
        {
            await appRoleService.AddRoleAsync(NewRoleName);

            ViewData["message"] = "Role added";
        }
        catch (Exception ex)
        {
            ViewData["message"] = ex.Message;
        }

        return RedirectToPage();
    }

    //public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        EmailMessageModel testEmail = new EmailMessageModel
    //        {
    //            Sender = new Email
    //            {
    //                Address = "MS_1puDAp@trial-z86org80oq04ew13.mlsender.net",
    //                Name = applicationTitle
    //            },
    //            Recipients = [new() { Address = Recipient }],
    //            Subject = "Test email message",
    //            Html = EmailBody
    //        };

    //        var emailServiceResponse = await emailService.SendEmailAsync(testEmail);

    //        ViewData["message"] = $"{emailServiceResponse}";
    //    }

    //    return Page();
    //}
}
