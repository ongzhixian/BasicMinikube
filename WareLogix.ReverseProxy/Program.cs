using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(httpLoggingOptions =>
{
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

const string serviceName = "ReverseProxy";

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        //.AddConsoleExporter()
        ;
});

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin();
    });
});

//const string serviceName = "yarpProxy";

//builder.Logging.AddOpenTelemetry(options =>
//{
//    options
//        .SetResourceBuilder(
//            ResourceBuilder.CreateDefault()
//                .AddService(serviceName))
//        .AddOtlpExporter();
//});

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(resource => resource.AddService(serviceName))
//    .WithTracing(tracing => tracing
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddSource("Yarp.ReverseProxy")
//        .AddOtlpExporter()
//    );


var app = builder.Build();

app.UseHttpLogging();

app.MapReverseProxy();

app.Run();