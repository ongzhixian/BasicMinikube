using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.Repositories;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

[Authorize]
public class AssignUserRolesPageModel : PageModel
{
    private readonly ILogger<AssignUserRolesPageModel> logger;
    private readonly AppRoleService appRoleService;
    private readonly AppUserService appUserService;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Role Name")]
    public string NewRoleName { get; set; } = string.Empty;

    public IEnumerable<string> AvailableRoles { get; set; } = Enumerable.Empty<string>();
    public long AvailableRolesCount { get; set; }

    public AssignUserRolesPageModel(ILogger<AssignUserRolesPageModel> logger, AppUserService appUserService, AppRoleService appRoleService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
        this.appRoleService = appRoleService;
    }

    public void OnGet()
    {
        //var getAllRolesTask = appRoleService.GetAllRolesAsync();
        //var getAllRolesCountTask = appRoleService.GetAllRolesCountAsync();

        //await Task.WhenAll(getAllRolesTask, getAllRolesCountTask);

        //AvailableRoles = getAllRolesTask.Result;
        //AvailableRolesCount = getAllRolesCountTask.Result;
    }

    public async Task<IActionResult> OnPost()
    {
        try
        {
            var selectedRoles = Request.Form["selectedRoles"].ToString().Split('|', StringSplitOptions.RemoveEmptyEntries);
            var selectedUsers = Request.Form["selectedUsers"].ToString().Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (var username in selectedUsers)
            {
                var user = await appUserService.GetUserAsync(username);

                if (user == null) continue;

                foreach (var roleName in selectedRoles)
                {
                    var role = await appRoleService.GetRoleAsync(roleName);

                    if (role == null) continue;

                    if (!user.Claims.Any(r => r.Type == ClaimTypes.Role && r.Value == role.RoleName))
                    {
                        var roleClaim = new Claim(ClaimTypes.Role, role.RoleName);
                        user.Claims.Add(roleClaim);
                    }
                }

                await appUserService.UpdateUserAsync(user);
            }

            ViewData["message"] = "User Role assigned";
        }
        catch (Exception ex)
        {
            ViewData["message"] = ex.Message;
        }

        //return RedirectToPage();
        return Page();
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
