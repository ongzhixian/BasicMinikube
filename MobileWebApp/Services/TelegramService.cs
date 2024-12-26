using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;

using MobileWebApp.Models;

namespace MobileWebApp.Services;

public sealed class TelegramService : IDisposable
{
    private bool disposedValue;

    private readonly ILogger<TelegramService> logger;
    private readonly HttpClient httpClient;

    private readonly string botToken;

    public TelegramService(ILogger<TelegramService> logger, IConfiguration configuration, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.botToken = configuration["telegram_bot_warelogix_token"] ?? throw new ConfigurationNullException("telegram_bot_warelogix_token");
    }


    public async Task SendMessageAsync(DraftMessage newMessage)
    {
        try
        {
            JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

            var json = JsonSerializer.Serialize(newMessage);

            StringContent content = new StringContent(
                json
                , System.Text.Encoding.UTF8
                , MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));

            var response = await httpClient.PostAsync($"/{botToken}/sendMessage", content, CancellationToken.None);

            var responseContent = await response.Content.ReadAsStringAsync();

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<HttpResponseMessage> GetMeAsync()
    {
        var response = await httpClient.GetAsync($"/{botToken}/getMe");

        var responseContent = await response.Content.ReadAsStringAsync();

        return response;
    }

    public async Task<HttpResponseMessage> GetWebhookAsync()
    {
        var response = await httpClient.GetAsync($"/{botToken}/getWebhookInfo");

        var responseContent = await response.Content.ReadAsStringAsync();

        return response;
    }


    public async Task SetWebhookAsync()
    {
        try
        {
            //var response = await httpClient.GetAsync($"/{botToken}/getMe");
            // https://frank-jaguar-frankly.ngrok-free.app/api/WareLogixBot

            WebHookParameters webHookParameters = new WebHookParameters();
            webHookParameters.url = "https://frank-jaguar-frankly.ngrok-free.app/api/WareLogixBot";

            System.Text.Json.JsonSerializerOptions jsonSerializerOptions = 
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);

            var json = System.Text.Json.JsonSerializer.Serialize(webHookParameters);

            StringContent content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(webHookParameters)
                , System.Text.Encoding.UTF8
                , System.Net.Http.Headers.MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Application.Json));

            //JsonContent jsonContent = JsonContent.Create()

            var response = await httpClient.PostAsync($"/{botToken}/setWebhook", content, CancellationToken.None);

            var responseContent = await response.Content.ReadAsStringAsync();

        }
        catch (Exception)
        {
            throw;
        }

        //try
        //{
        //    var response = await httpClient.PostAsync("https://api.mailersend.com/v1/email", emailMessage.ToStringContent());

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    // Consider passing the x-message-id in response
        //    //if (response.Headers.Contains("x-message-id"))
        //    //{
        //    //    var messageId = response.Headers.GetValues("x-message-id");
        //    //}

        //    if (response.IsSuccessStatusCode && string.IsNullOrEmpty(responseContent))
        //        return EmailServiceResponse.Success;

        //    logger.LogInformation("{EmailApiResponseContent}", responseContent);

        //    if (response.IsSuccessStatusCode)
        //        return EmailServiceResponse.SuccessWithErrors;

        //    return EmailServiceResponse.FailureWithErrors;
        //}
        //catch (Exception ex)
        //{
        //    logger.LogError("Error sending e-mail: {Error}", ex);
        //}

        //return EmailServiceResponse.Failure;
    }

    // DISPOSE 

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                httpClient?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~TelegramService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    
}


public class WebHookParameters
{
    public string url { get; set; }
}
