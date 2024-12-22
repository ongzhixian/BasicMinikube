using System.Net;
using System.Net.WebSockets;
using System.Text;

using Microsoft.AspNetCore.Http.HttpResults;

var connections = new HashSet<WebSocket>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.MapGet("/health", () =>
{
    return TypedResults.Ok("OK");
});

app.Map("/wsa", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        Console.WriteLine("Received web socket request");

        var count = 0;
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        connections.Add(ws);

        await Broadcast($"join the room");
        await Broadcast($"Number of users: {connections.Count}");

        await ReceiveMessage(ws, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await Broadcast(message);
            }
            else if (result.MessageType == WebSocketMessageType.Close 
            || ws.State == WebSocketState.Aborted) 
            {
                connections.Remove(ws);
                await Broadcast("Someone left");
                await ws.CloseAsync(result.CloseStatus.Value
                    , result.CloseStatusDescription
                    , CancellationToken.None);
            }
        });

        //while (true)
        //{
        //    var message = $"Hello {count++}";
            
        //    if (ws.State == System.Net.WebSockets.WebSocketState.Open)
        //    {
        //        Console.WriteLine($"SEND MESSAGE: {message}");
        //        await ws.SendAsync(
        //            new ArraySegment<byte>(Encoding.UTF8.GetBytes(message))
        //            , System.Net.WebSockets.WebSocketMessageType.Text
        //            , true
        //            , CancellationToken.None);
        //    }
        //    else if ((ws.State == WebSocketState.Closed) || (ws.State == WebSocketState.Aborted))
        //    {
        //        break;
        //    }
        //    else
        //    {
        //        Console.WriteLine($"wsState is {ws.State}");
        //    }

        //    Thread.Sleep(1000);
        //}
    }
    else
    {
        Console.WriteLine("Received BAD web socket request");
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

//app.Run();
await app.RunAsync();

return; 

async Task Broadcast(string message)
{
    var bytes = Encoding.UTF8.GetBytes(message);

    foreach (var socket in connections)
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(arraySegment
                , WebSocketMessageType.Text
                , true
                , CancellationToken.None);
        }
    }
}

async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer)
            , CancellationToken.None);

        handleMessage(result, buffer);
    }
}
