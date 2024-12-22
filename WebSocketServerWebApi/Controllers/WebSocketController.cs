using System.Net.WebSockets;
using System.Net;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Primitives;
using System.Net.Sockets;

namespace WebSocketServerWebApi.Controllers;

[ApiController]
[Route("ws")]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> logger;

    private readonly static HashSet<WebSocket> connections = [];

    public WebSocketController(ILogger<WebSocketController> logger)
    {
        this.logger = logger;
    }

    [Route("/ws")] // To handle CONNECT (HTTP/2)
    [HttpGet("/ws", Name = "Connect")]
    public async Task GetAsync()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            logger.LogInformation("Received BAD web socket request");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        logger.LogDebug("TODO: Some validation here");

        var username = HttpContext.Request.Query["username"].ToString() ?? 
            Guid.NewGuid().ToString().Substring(0, 8);

        logger.LogInformation($"{username} connected to web socket server");

        using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
        connections.Add(ws);

        await Broadcast($"{username} join the room");
        await Broadcast($"Number of users in room: {connections.Count}");

        await HandleReceivedMessagesFor(ws, username, ReceivedMessageHandlerAsync);

        //await ReceiveMessage(ws, async (result, buffer) =>
        //{
        //    if (result.MessageType == WebSocketMessageType.Text)
        //    {
        //        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //        await Broadcast(message);
        //    }
        //    else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
        //    {
        //        connections.Remove(ws);
        //        await Broadcast($"{username} left");
        //        await ws.CloseAsync(result.CloseStatus.Value
        //            , result.CloseStatusDescription
        //            , CancellationToken.None);
        //    }
        //});

    }

    private async Task ReceivedMessageHandlerAsync(WebSocket socket, string username, WebSocketReceiveResult result, byte[] buffer)
    {
        if (result.MessageType == WebSocketMessageType.Text)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await Broadcast(message);
        }
        else if (result.MessageType == WebSocketMessageType.Close || socket.State == WebSocketState.Aborted)
        {
            connections.Remove(socket);
            await Broadcast($"{username} left");
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure
                , result.CloseStatusDescription
                , CancellationToken.None);
        }
    }

    //private async Task ReceivedMessageHandlerAsync(WebSocket socket, string username, WebSocketReceiveResult result, byte[] buffer)
    //{
    //    if (result.MessageType == WebSocketMessageType.Text)
    //    {
    //        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
    //        await Broadcast(message);
    //    }
    //    else if (result.MessageType == WebSocketMessageType.Close || socket.State == WebSocketState.Aborted)
    //    {
    //        connections.Remove(socket);
    //        await Broadcast($"{username} left");
    //        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure
    //            , result.CloseStatusDescription
    //            , CancellationToken.None);
    //    }
    //}

    private async Task HandleReceivedMessagesFor(WebSocket socket, string username, 
        Func<WebSocket, string, WebSocketReceiveResult, byte[], Task> messageHandlerFor)
    {
        var bufferStore = new byte[1024];
        var receiveBuffer = new ArraySegment<byte>(bufferStore);

        while (socket.State == WebSocketState.Open)
        {
            try
            {
                var result = await socket.ReceiveAsync(receiveBuffer, CancellationToken.None);

                await messageHandlerFor(socket, username, result, bufferStore);
            }
            catch (WebSocketException exception)
            {
                if (exception.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                {
                    connections.Remove(socket);
                    await Broadcast($"{username} left");
                    // Duh! Its already closed! :-D
                    //await socket.CloseAsync(WebSocketCloseStatus.NormalClosure
                    //    , WebSocketError.ConnectionClosedPrematurely.ToString()
                    //    , CancellationToken.None);
                    break;
                };

                throw;
            }
        }
    }


    // PRIVATE METHODS

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
            try
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                handleMessage(result, buffer);
            }
            catch (WebSocketException exception)
            {
                if (exception.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) break;

                throw;
            }
        }
    }

}
