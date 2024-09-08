namespace TradingConsoleApp.Services;

using System.Diagnostics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


internal class JaegerDemoService : BackgroundService
{
    private readonly ILogger<JaegerDemoService> logger;

    public JaegerDemoService(ILogger<JaegerDemoService> logger)
    {
        this.logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"bean {i++}");

            ActivitySource MyActivitySource = new("OpenTelemetry.Demo.Jaeger");

            using var parent = MyActivitySource.StartActivity("JaegerDemo");

            using (var client = new HttpClient())
            {
                using (var slow = MyActivitySource.StartActivity("SomethingSlow"))
                {
                    await client.GetStringAsync("https://httpstat.us/200?sleep=1000");
                    await client.GetStringAsync("https://httpstat.us/200?sleep=1000");
                }

                using (var fast = MyActivitySource.StartActivity("SomethingFast"))
                {
                    await client.GetStringAsync("https://httpstat.us/301");
                }
            }

            this.logger.LogInformation("JaegerDemoService running at: {time}", DateTimeOffset.Now);
            await Task.Delay(3600, stoppingToken);
            
            //Thread.Sleep(3600);
            //await Task.Delay(1000, stoppingToken);
        }
        //Once execution leaves this method, it will not be called again.
    }
}
