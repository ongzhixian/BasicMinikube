using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace TradingBot.Services;

internal class ChatHubHeartbeatService : BackgroundService
{
    HubConnection connection;

    public ChatHubHeartbeatService()
    {
        connection = new HubConnectionBuilder()
               .WithUrl("https://localhost:5207/chathub")
               .Build();

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            await SendMessageAsync();
            await Task.Delay(1000);
        }
    }

    public async Task ConnectAsync()
    {
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            //this.Dispatcher.Invoke(() =>
            //{
            //    var newMessage = $"{user}: {message}";
            //    messagesList.Items.Add(newMessage);
            //});

            Console.WriteLine(message);
        });

        try
        {
            await connection.StartAsync();
            //messagesList.Items.Add("Connection started");
            //connectButton.IsEnabled = false;
            //sendButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            //messagesList.Items.Add(ex.Message);
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SendMessageAsync()
    {
        try
        {
            await connection.InvokeAsync("SendMessage", "TradingBot", "Trading bot says hello");
        }
        catch (Exception ex)
        {
            //messagesList.Items.Add(ex.Message);
            Console.WriteLine(ex.Message);
        }
    }


}
