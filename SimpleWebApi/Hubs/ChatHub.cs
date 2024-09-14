using Microsoft.AspNetCore.SignalR;

namespace SimpleWebApi.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        var mr = new MessageRecord(DateTime.Now, user, message);
        
        await Clients.All.SendAsync("ReceiveMessage", user, mr);
    }
        

    //public Task SendMessage(string user, string message,
    //   [FromServices] IDatabaseService dbService)
    //{
    //    var userName = dbService.GetUserName(user);
    //    return Clients.All.SendAsync("ReceiveMessage", userName, message);
    //}
}


public record MessageRecord(DateTime timestamp, string user, string content);

//public record MessageRecord()
//{
//    public DateTime Timestamp { get; init; }
//    public string User { get; init; }
//    public string Content { get; init; }
//}