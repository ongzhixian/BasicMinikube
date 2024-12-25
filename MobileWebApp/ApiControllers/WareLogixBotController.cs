using System.Text.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Api;
using MobileWebApp.Models;
using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.ApiControllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class WareLogixBotController : ControllerBase
{
    private readonly ILogger<WareLogixBotController> logger;
    //private readonly AppUserService appUserService;
    private readonly TelegramService telegramService;

    public WareLogixBotController(ILogger<WareLogixBotController> logger, TelegramService telegramService)
    {
        this.logger = logger;
        this.telegramService = telegramService;
    }

    // GET: api/<WareLogixBotController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // POST api/<WareLogixBotController>
    [HttpPost]
    public async Task<Microsoft.AspNetCore.Http.HttpResults.Ok> PostAsync()
    {
        //var bodyStream = new StreamReader(Request.Body);
        //var bodyText = await bodyStream.ReadToEndAsync();

        var update = await JsonSerializer.DeserializeAsync<TelegramUpdate>(Request.Body);

        // Analyze update

        DraftMessage newMessage = new DraftMessage();
        newMessage.chat_id = update.message.chat.id;
        newMessage.text = $"Echo -- {update.message.text}";

        await telegramService.SendMessageAsync(newMessage);

        return TypedResults.Ok();
    }
}
