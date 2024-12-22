using System.Net;
using System.Net.WebSockets;
using System.Text;

string username = Guid.NewGuid().ToString().Substring(0, 8);

if (args.Length > 0)
    username = args[0];

using var ws = new ClientWebSocket();
ws.Options.SetRequestHeader("SomeHeader1", "SomeHeader1Value");
ws.Options.SetRequestHeader("SomeHeader2", null);
ws.Options.Credentials = new System.Net.NetworkCredential("SomeUsername", "SomePassword", "SomeDomain");
ws.Options.UseDefaultCredentials = true;
//ws.Options.Proxy = new WebProxy("localhost", 8083);

//HttpClientHandler handler = new HttpClientHandler();
//handler.Credentials = new System.Net.NetworkCredential("SomeUsername", "SomePassword", "SomeDomain");
//handler.Properties.Add("Hello", "workd");
//handler.Proxy = new WebProxy("localhost", 8083);
//handler.PreAuthenticate = true;
//HttpMessageInvoker httpMessageInvoker = new HttpMessageInvoker(handler);

Console.WriteLine($"Connecting to web socket server as {username}...");
await ws.ConnectAsync(
    new Uri($"wss://localhost:5041/ws?username={username}")
    //, httpMessageInvoker
    , CancellationToken.None);
Console.WriteLine("Connected");

// Setup 2 tasks: 1 for receiving messages and 1 for sending messages
var receiveTask = ReceiveMessagesTaskAsync();

var sendTask = SendMessagesTaskAsync();

await Task.WhenAny(receiveTask, sendTask);

if (ws.State != WebSocketState.Closed)
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

await Task.WhenAll(receiveTask, sendTask);

return;

async Task SendMessagesTaskAsync()
{
    while (true)
    {
        var message = Console.ReadLine() ?? string.Empty;

        if (message == "exit")
            break;

        await ws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

async Task ReceiveMessagesTaskAsync()
{
    var byteArray = new byte[1024];
    var buffer = new ArraySegment<byte>(byteArray);

    while (true)
    {
        var webSocketReceiveResult = await ws.ReceiveAsync(buffer, CancellationToken.None);

        if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
            break;

        var message = Encoding.UTF8.GetString(byteArray, 0, webSocketReceiveResult.Count);
        Console.WriteLine($"Received {message}");
    }
}