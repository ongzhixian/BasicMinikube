using MobileWebApp.Models;

namespace MobileWebApp.Services;

/// <summary>
///  This is email service modeled after MailerSend
///  Should 1: compare it against some other service provider as SendGrid, MailChimp
///  Should 2: Put in some standard library
/// </summary>
public sealed class EmailService : IDisposable
{
    private bool disposedValue;
    private readonly ILogger<EmailService> logger;
    private readonly HttpClient httpClient;

    public EmailService(ILogger<EmailService> logger, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;
    }

    public async Task<EmailServiceResponse> SendEmailAsync(EmailMessageModel emailMessage)
    {
        try
        {
            var response = await httpClient.PostAsync("https://api.mailersend.com/v1/email", emailMessage.ToStringContent());

            var responseContent = await response.Content.ReadAsStringAsync();

            // Consider passing the x-message-id in response
            //if (response.Headers.Contains("x-message-id"))
            //{
            //    var messageId = response.Headers.GetValues("x-message-id");
            //}

            if (response.IsSuccessStatusCode && string.IsNullOrEmpty(responseContent))
                return EmailServiceResponse.Success;

            logger.LogInformation("{EmailApiResponseContent}", responseContent);

            if (response.IsSuccessStatusCode)
                return EmailServiceResponse.SuccessWithErrors;

            return EmailServiceResponse.FailureWithErrors;
        }
        catch (Exception ex)
        {
            logger.LogError("Error sending e-mail: {Error}", ex);
        }

        return EmailServiceResponse.Failure;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                httpClient?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~EmailService()
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


public enum EmailServiceResponse : byte
{
    Success = 0x0,              // 0x0  : 0000
    SuccessWithErrors = 0x1,    // 0x1  : 0001
    Failure = 0x2,              // 0x2  : 0010
    FailureWithErrors = 0x3     // 0x3  : 0011
}


//public sealed class TodoService(
//    HttpClient httpClient,
//    ILogger<TodoService> logger) : IDisposable
//{
//    public async Task<Todo[]> GetUserTodosAsync(int userId)
//    {
//        try
//        {
//            // Make HTTP GET request
//            // Parse JSON response deserialize into Todo type
//            Todo[]? todos = await httpClient.GetFromJsonAsync<Todo[]>(
//                $"todos?userId={userId}",
//                new JsonSerializerOptions(JsonSerializerDefaults.Web));

//            return todos ?? [];
//        }
//        catch (Exception ex)
//        {
//            logger.LogError("Error getting something fun to say: {Error}", ex);
//        }

//        return [];
//    }

//    public void Dispose() => httpClient?.Dispose();
//}
