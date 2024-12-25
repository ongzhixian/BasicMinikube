using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class TelegramBotsPageModel : PageModel
{
    private readonly ILogger<TelegramBotsPageModel> logger;
    private TelegramService telegramService;

    public TelegramBotsPageModel(ILogger<TelegramBotsPageModel> logger, TelegramService telegramService)
    {
        this.logger = logger;
        this.telegramService = telegramService;
    }

    public void OnGet()
    {

    }

    public async Task OnPostAsync([FromForm]string action)
    {
        //var emailAddress = Request.Form["emailaddress"];
        var url = "https://frank-jaguar-frankly.ngrok-free.app";

        switch (action)
        {
            case "SetWebhook":
                await telegramService.SetWebhookAsync();
                break;
            case "GetMe":
                await telegramService.GetMeAsync();
                break;
            case "GetWebhook":
                await telegramService.GetWebhookAsync();
                break;
        }

    }
}
