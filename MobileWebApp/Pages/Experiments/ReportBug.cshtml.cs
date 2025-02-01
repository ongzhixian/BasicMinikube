using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

[Authorize]
public class ReportBugPageModel : PageModel
{
    private readonly string applicationTitle;
    private readonly ILogger<ReportBugPageModel> logger;
    private readonly EmailService emailService;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Recipient(e-mail address)")]
    public string Recipient { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Message content (HTML)")]
    public string EmailBody { get; set; } = string.Empty;

    public ReportBugPageModel(ILogger<ReportBugPageModel> logger, IConfiguration configuration, EmailService emailService)
    {
        this.logger = logger;
        this.emailService = emailService;
        applicationTitle = configuration["Application:Title"] ?? AppDomain.CurrentDomain.FriendlyName;

        Recipient = "zhixian@hotmail.com";
        EmailBody = "<p>Some sample content</p>";
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            EmailMessageModel testEmail = new EmailMessageModel
            {
                Sender = new Email
                {
                    Address = "MS_1puDAp@trial-z86org80oq04ew13.mlsender.net",
                    Name = applicationTitle
                },
                Recipients = [new() { Address = Recipient }],
                Subject = "Test email message",
                Html = EmailBody
            };

            var emailServiceResponse = await emailService.SendEmailAsync(testEmail);

            ViewData["message"] = $"{emailServiceResponse}";
        }

        return Page();
    }
}
