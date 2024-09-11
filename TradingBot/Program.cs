using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;
using Polly.Timeout;

using Serilog;

using TradingConsoleApp.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("logs/root.logs", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Debug("TradingBot bootstrapped");

try
{
    HostApplicationBuilder builder = GetApplicationBuilder(args);

    Log.Debug("=== Dumping configuration ===");
    Log.Debug("Key                      Value");
    Log.Debug($"Oanda:RestApiUrl        {builder.Configuration["Oanda:RestApiUrl"]}");
    Log.Debug($"Oanda:StreamingApiUrl   {builder.Configuration["Oanda:StreamingApiUrl"]}");

    ConfigureLogging(builder);
    AddHttpResilience(builder);
    AddHttpServices(builder);

    //builder.Services.AddSingleton<OandaService>();
    //builder.Services.AddSingleton<IMessageQueuePublisher, RabbitMqPublisher>(sp =>
    //{
    //    return new RabbitMqPublisher(builder.Configuration);
    //});
    //builder.Services.AddHostedService<JaegerDemoService>();
    //builder.Services.AddHostedService<SimpleApiDemoService>();
    //builder.Services.AddHostedService<CloudAmqpDemoService>();
    //ilder.Services.AddHostedService<Worker>();
    builder.Services.AddSingleton<OandaStreamingService>();

    var serviceName = "TradingBot";

    using IHost host = builder.Build();
    
    var tradingAccountId = builder.Configuration["Oanda:TradingAccountId"];
    var oandaStreamingService = host.Services.GetRequiredService<OandaStreamingService>();
    await oandaStreamingService.GetPriceStreamAsync(tradingAccountId);


    host.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "Unhandled exception!");
}
finally
{
    await Log.CloseAndFlushAsync();
}

return;

static HostApplicationBuilder GetApplicationBuilder(string[] args)
{
    // What `CreateApplicationBuilder` does:
    // Sets the content root to the path returned by GetCurrentDirectory().
    // Loads host configuration from:
    //   Environment variables prefixed with DOTNET_.
    //   Command - line arguments.
    // Loads app configuration from:
    //   appsettings.json.
    //   appsettings.{ Environment}.json.
    //   Secret Manager when the app runs in the Development environment.
    //   Environment variables.
    //   Command - line arguments.
    // Adds the following logging providers:
    //   Console
    //   Debug
    //   EventSource
    //   EventLog(only when running on Windows)
    // Enables scope validation and dependency validation when the environment is Development.
    // Reference: https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder

    Log.Debug("Create application builder");
    return Host.CreateApplicationBuilder(args);
}

static void ConfigureLogging(HostApplicationBuilder builder)
{
    builder.Logging.ClearProviders();

    builder.Logging.AddSerilog(new LoggerConfiguration()
        .ReadFrom
        .Configuration(builder.Configuration)
        .CreateLogger());

    Log.Debug("Logging configured");
}

static void AddHttpResilience(HostApplicationBuilder builder)
{
    // HTTP Resiliency
    // Reference: https://learn.microsoft.com/en-us/dotnet/core/resilience/?tabs=dotnet-cli

    const string resiliencePipelineName = "Retry-Timeout";

    builder.Services.AddResiliencePipeline(resiliencePipelineName, static builder =>
    {
        // See: https://www.pollydocs.org/strategies/retry.html
        builder.AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>()
        });

        // See: https://www.pollydocs.org/strategies/timeout.html
        builder.AddTimeout(TimeSpan.FromSeconds(1.5));
    });

    builder.Services.AddResilienceEnricher();
}


static void AddHttpServices(HostApplicationBuilder builder)
{
    //builder.Services.AddHttpClient();
    builder.Services.AddHttpClient();
    

    //// Reference: https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli
    //builder.Services
    //    .AddHttpClient<OandaService>()
    //    .AddStandardResilienceHandler(options =>
    //    {
    //        options.Retry.MaxRetryAttempts = 3;
    //    });

    // See also the many fun topics about HTTP, eg.
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-sni
}



//while (true)
//{
    
//    //var data = await GetMarketData(apiUrl, apiKey);
//    //var signal = GenerateSignal(data);

//    //if (signal == "BUY")
//    //{
//    //    PlaceOrder("BUY");
//    //}
//    //else if (signal == "SELL")
//    //{
//    //    PlaceOrder("SELL");
//    //}

//    await Task.Delay(60000); // Wait for 1 minute before next check
//}


//static async Task<JObject> GetMarketData(string apiUrl, string apiKey)
//{
//    using (HttpClient client = new HttpClient())
//    {
//        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
//        var response = await client.GetStringAsync(apiUrl);
//        return JObject.Parse(response);
//    }
//}

//static string GenerateSignal(JObject data)
//{
//    // Implement your trading logic here
//    // For example, a simple moving average crossover strategy
//    var prices = data["prices"].ToObject<double[]>();
//    double shortTermMA = CalculateMovingAverage(prices, 5);
//    double longTermMA = CalculateMovingAverage(prices, 20);

//    if (shortTermMA > longTermMA)
//    {
//        return "BUY";
//    }
//    else if (shortTermMA < longTermMA)
//    {
//        return "SELL";
//    }
//    return "HOLD";
//}

//static double CalculateMovingAverage(double[] prices, int period)
//{
//    double sum = 0;
//    for (int i = 0; i < period; i++)
//    {
//        sum += prices[i];
//    }
//    return sum / period;
//}

static void PlaceOrder(string orderType)
{
    // Implement your order placement logic here
    Console.WriteLine($"Placing {orderType} order.");
}