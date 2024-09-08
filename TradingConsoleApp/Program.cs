using System;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



using Polly;
using Polly.Retry;
using Polly.Timeout;

using RabbitMQ.Client;

//using OpenTelemetry.Logs;
//using OpenTelemetry.Metrics;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;

using Serilog;

using TradingConsoleApp.Models.OandaApi;
using TradingConsoleApp.Services;
using System.Diagnostics;
using OpenTelemetry.Resources;
using WareLogix.Telemetry;
using OpenTelemetry.Exporter;
using WareLogix.Messaging;
using WareLogix.Messaging.RabbitMq;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("logs/root.logs", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Debug("TradingConsoleApp bootstrapped");

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

    AddHttpResilience(builder);

    //builder.Services.AddSingleton<OandaService>();

    AddHttpServices(builder);

    builder.Services.AddSingleton<IMessageQueuePublisher, RabbitMqPublisher>(sp =>
    {
        return new RabbitMqPublisher(builder.Configuration);
    });
    //builder.Services.AddHostedService<JaegerDemoService>();
    //builder.Services.AddHostedService<SimpleApiDemoService>();
    builder.Services.AddHostedService<CloudAmqpDemoService>();

    var serviceName = "TradingConsoleApp";
    
    SetupOpenTelemetry(builder, serviceName);

    //builder.Services.AddHostedService<Worker>();

    using (IHost host = builder.Build())
    {
        var oandaService = host.Services.GetRequiredService<OandaService>();
        if (oandaService != null)
        {
            var tradingAccountId = builder.Configuration["Oanda:TradingAccountId"];

            //await oandaService.GetAccountsAsync();
            //await oandaService.GetAccountSummaryAsync(tradingAccountId);
            //await oandaService.GetAccountTradableInstrumentsAsync(tradingAccountId);

            //await oandaService.GetInstrumentCandlesAsync("USD_CNH");
            //await oandaService.GetInstrumentOrderBookAsync("XAU_USD");
            //await oandaService.GetInstrumentPositionBookAsync("XAU_USD");

            //{
            //    "order": {
            //        "units": "100",
            //        "instrument": "EUR_USD",

            //        "timeInForce": "FOK",
            //        "type": "MARKET",
            //        "positionFill": "DEFAULT"
            //    }
            //}
            //LimitOrder orderRequest = new LimitOrder
            //{
            //    type = "LIMIT",
            //    units = "100",
            //    price = "200",
            //    instrument = "XAU_USD",
            //    timeInForce = "FOK",
            //    positionFill = "DEFAULT"
            //};
            //await oandaService.CreateOrderAsync(tradingAccountId, orderRequest);
            //await oandaService.CancelPendingOrderAsync(tradingAccountId, orderRequest);
            //await oandaService.GetOrdersAsync(tradingAccountId);

            //using var source = new ActivitySource("Samples.SampleServer");
            //using var activity = source.StartActivity("beancounter", ActivityKind.Server);
            //for (int i = 0; i < 999; i++)
            //{
            //    activity?.SetTag($"BeanCounter", i);
            //    Console.WriteLine($"Count {i}");
            //    Thread.Sleep(1250);
            //}




            Log.Information("Startup completed");
        }

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
    builder.Services.AddHttpClient();

    // Reference: https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli
    builder.Services
        .AddHttpClient<OandaService>()
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
        });

    // See also the many fun topics about HTTP, eg.
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-sni
}

static void SetupOpenTelemetry(HostApplicationBuilder builder, string serviceName)
{
    // What's next
    // Send to Splunk
    // Send to Grafana / Grafana cloud

    //WareLogix.Telemetry.TelemetryConfiguration.AddOpenTelemetry(builder.Configuration, builder.Services);
    var otel = WareLogix.Telemetry.TelemetryConfiguration.GetOpenTelemetryBuilder(builder.Services);
    WareLogix.Telemetry.TelemetryConfiguration.DefineResourceService(otel, serviceName + "METHD", "1.0.1");
    WareLogix.Telemetry.TelemetryConfiguration.AddTracing(otel);
    WareLogix.Telemetry.TelemetryConfiguration.UseOtlpExporter(otel);
    // Set via Options API using code:
    //builder.Services.Configure<OtlpExporterOptions>(o => {
    //    // ...o.Headers
    //});


    // Using extension methods (requires add namespaces `using WareLogix.Telemetry;`)
    //otel.DefineResourceService(serviceName + "EXTMETH", "1.0.1")
    //    .AddTracing()
    //    .UseOtlpExporter()
    //    ;

    //otel.DefineResourceService(serviceName, "1.0.1");


    // WORKS
    //using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    //    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
    //        serviceName: "DemoApp",
    //        serviceVersion: "1.0.0"))
    //    .AddSource("OpenTelemetry.Demo.Jaeger")
    //    //.AddHttpClientInstrumentation()
    //    .AddConsoleExporter()
    //    .AddOtlpExporter(opt =>
    //    {
    //        opt.Endpoint = new Uri("http://localhost:4317");
    //    })
    //    .Build();
}