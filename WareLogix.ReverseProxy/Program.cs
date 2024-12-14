using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using Yarp.ReverseProxy.Forwarder;
using System.Diagnostics;
using System.Net;
using System.Collections.Immutable;
using WareLogix.ReverseProxy;

const string serviceName = "WareLogixProxy";

ImmutableArray<string>? appAuthorities = null;

AppConfig appConfig = new();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection("AppConfig").Bind(appConfig);

ConfigureLogging(builder.Logging);

ConfigureHttpLogging(builder.Services);

ConfigureOpenTelemetry(builder.Services);

ConfigureCorsPolicy(builder.Services);

if (appConfig.IsHttpForwarder)
    builder.Services.AddHttpForwarder();
else
    ConfigureReverseProxy(appConfig, builder);

// RUN APPLICATION

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(InitializeApplication);

app.UseHttpLogging();

if (appConfig.IsHttpForwarder)
    app.Map(pattern: "{**catch-all}", ForwardExternalRequests(appAuthorities));
else
    app.MapReverseProxy();

app.Run();

return;

void ConfigureLogging(ILoggingBuilder logging)
{
    logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            //.AddConsoleExporter()
            ;
    });

    //builder.Logging.AddOpenTelemetry(options =>
    //{
    //    options
    //        .SetResourceBuilder(
    //            ResourceBuilder.CreateDefault()
    //                .AddService(serviceName))
    //        .AddOtlpExporter();
    //});

}

void ConfigureHttpLogging(IServiceCollection services)
{
    services.AddHttpLogging(httpLoggingOptions =>
    {
    });
}

void ConfigureOpenTelemetry(IServiceCollection services)
{
    builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddSource("Yarp.ReverseProxy")
          //.AddConsoleExporter()
          .AddOtlpExporter(opt =>
          {
              opt.Endpoint = new Uri("http://localhost:4317");
              //opt.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
          })
      //).WithMetrics(metrics => metrics
      //    .AddAspNetCoreInstrumentation()
      //    .AddHttpClientInstrumentation()
      //    .AddConsoleExporter()
      //    .AddOtlpExporter(opt =>
      //    {
      //        opt.Endpoint = new Uri("http://localhost:4317");
      //    })
          );

    //builder.Services.AddOpenTelemetry()
    //    .ConfigureResource(resource => resource.AddService(serviceName))
    //    .WithTracing(tracing => tracing
    //        .AddAspNetCoreInstrumentation()
    //        .AddHttpClientInstrumentation()
    //        .AddSource("Yarp.ReverseProxy")
    //        .AddOtlpExporter()
    //    );

}

void ConfigureCorsPolicy(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("customPolicy", builder =>
        {
            builder.AllowAnyOrigin();
        });
    });
}

bool RequestTargetSelf(string requestAuthority) => appAuthorities!.Contains(requestAuthority);

void InitializeApplication()
{
    // Do any startup tasks here post-app.Run() 
    if (appAuthorities == null)
        appAuthorities = app.Urls.Select(url => new Uri(url).Authority).ToImmutableArray();
}

Func<HttpContext, IHttpForwarder, Task> ForwardExternalRequests(ImmutableArray<string>? appAuthorities)
{
    return async (HttpContext httpContext, IHttpForwarder forwarder) =>
    {
        if (RequestTargetSelf(httpContext.Request.Host.Value))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return;
        }

        using var httpClient = new HttpMessageInvoker(new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            EnableMultipleHttp2Connections = true,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15),
        });

        var error = await forwarder.SendAsync(httpContext, $"{httpContext.Request.Scheme}://{httpContext.Request.Host}", httpClient);

        if (error != ForwarderError.None) // Check if the proxy operation was successful
        {
            var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
            var exception = errorFeature?.Exception;
        }
    };
}

static void ConfigureReverseProxy(AppConfig appConfig, WebApplicationBuilder builder)
{
    var reverseProxy = builder.Services.AddReverseProxy();
    foreach (var item in appConfig.ReverseProxyRoutesConfigs)
        reverseProxy.LoadFromConfig(builder.Configuration.GetSection(item));
}