namespace TradingConsoleApp.Services;

using System.Diagnostics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


internal class SimpleApiDemoService : BackgroundService
{
    private readonly ILogger<SimpleApiDemoService> logger;

    public SimpleApiDemoService(ILogger<SimpleApiDemoService> logger)
    {
        this.logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 0;
        Uri slowUrl = new Uri("https://localhost:5207/200?sleep=1000");
        Uri fastUrl = new Uri("https://localhost:5207/200");

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"bean {i++}");

            ActivitySource MyActivitySource = new("OpenTelemetry.Demo.SimpleApi");

            using var parent = MyActivitySource.StartActivity("SimpleApi");

            using (var client = new HttpClient())
            {
                using (var slow = MyActivitySource.StartActivity("SomethingSlow"))
                {
                    await client.GetStringAsync(slowUrl);
                    await client.GetStringAsync(slowUrl);
                }

                using (var fast = MyActivitySource.StartActivity("SomethingFast"))
                {
                    await client.GetStringAsync(fastUrl);
                }
            }

            this.logger.LogInformation("SimpleApiDemoService running at: {time}", DateTimeOffset.Now);
            await Task.Delay(3600, stoppingToken);
            
            //Thread.Sleep(3600);
            //await Task.Delay(1000, stoppingToken);
        }
        //Once execution leaves this method, it will not be called again.
    }
}
