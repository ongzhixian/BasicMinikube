using System.Data.Common;
using System;
using System.Text;
using System.Threading.Channels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Serilog;
using TechnicalAnalysisConsoleApp.Services;
using WareLogix.Messaging.RabbitMq;
using WareLogix.Messaging;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("logs/root.logs", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Debug("TechnicalAnalysisConsoleApp bootstrapped");

try
{
    HostApplicationBuilder builder = GetApplicationBuilder(args);

    //Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate
    //Log.Debug($"USE_DUMMY_FLAG {builder.Configuration["FeatureFlags:USE_DUMMY_FLAG"]}");
    //Log.Debug($"OandaAccessToken {builder.Configuration["OandaAccessToken"]}");
    Log.Debug("=== Dumping configuration ===");
    Log.Debug("Key                     Value");
    Log.Debug($"Oanda:RestApiUrl        {builder.Configuration["Oanda:RestApiUrl"]}");
    Log.Debug($"Oanda:StreamingApiUrl   {builder.Configuration["Oanda:StreamingApiUrl"]}");

    ConfigureLogging(builder);

    //builder.Services.AddOptions<MyOptions>().BindConfiguration("MyConfig");

    //AddHttpResilience(builder);

    //builder.Services.AddSingleton<OandaService>();

    //AddHttpServices(builder);

    builder.Services.AddSingleton<IMessageQueueConsumer<string>, RabbitMqConsumer>(sp =>
    {
        return new RabbitMqConsumer(builder.Configuration);
    });

    builder.Services.AddSingleton<TradableInstrumentService>();
    //builder.Services.AddHostedService<Worker>();
    builder.Services.AddHostedService<CloudAmqpDemoService>();

    using (IHost host = builder.Build())
    {
        host.Run();
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Unhandled exception!");
}
finally
{
    await Log.CloseAndFlushAsync();
}

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


// METHODS TO DUMP TO ConnectHttp class library

//static void AddHttpResilience(HostApplicationBuilder builder)
//{
//    // HTTP Resiliency
//    // Reference: https://learn.microsoft.com/en-us/dotnet/core/resilience/?tabs=dotnet-cli

//    const string resiliencePipelineName = "Retry-Timeout";

//    builder.Services.AddResiliencePipeline(resiliencePipelineName, static builder =>
//    {
//        // See: https://www.pollydocs.org/strategies/retry.html
//        builder.AddRetry(new RetryStrategyOptions
//        {
//            ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>()
//        });

//        // See: https://www.pollydocs.org/strategies/timeout.html
//        builder.AddTimeout(TimeSpan.FromSeconds(1.5));
//    });

//    builder.Services.AddResilienceEnricher();
//}

//static void AddHttpServices(HostApplicationBuilder builder)
//{
//    builder.Services.AddHttpClient();

//    // Reference: https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli
//    builder.Services
//        .AddHttpClient<OandaService>()
//        .AddStandardResilienceHandler(options =>
//        {
//            options.Retry.MaxRetryAttempts = 3;
//        });

//    // See also the many fun topics about HTTP, eg.
//    // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-sni
//}