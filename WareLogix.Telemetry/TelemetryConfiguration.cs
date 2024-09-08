
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WareLogix.Telemetry;

public sealed class TelemetryConfiguration
{
    // A more piece-meal approach; breaking the steps down

    public static OpenTelemetryBuilder GetOpenTelemetryBuilder(IServiceCollection services)
    {
        return services.AddOpenTelemetry();
    }

    public static void DefineResourceService(OpenTelemetryBuilder openTelemetryBuilder, string serviceName, string serviceVersion)
    {
        openTelemetryBuilder.ConfigureResource(builder =>
        {
            builder.AddService(serviceName: serviceName, serviceVersion: serviceVersion);
        });
    }

    public static void AddTracing(OpenTelemetryBuilder openTelemetryBuilder, bool useConsoleExporterForTrace = false, params string[] activitySourceNames)
    {

        openTelemetryBuilder.WithTracing(builder =>
        {
            if (activitySourceNames.Length > 0)
                builder.AddSource(activitySourceNames);
            else
                builder.AddSource("*");

            if (useConsoleExporterForTrace) builder.AddConsoleExporter();
        });
    }

    public static void AddTracing(OpenTelemetryBuilder openTelemetryBuilder, bool useConsoleExporterForTrace = false) => 
        AddTracing(openTelemetryBuilder, useConsoleExporterForTrace, activitySourceNames: "*");

    public static void UseOtlpExporter(OpenTelemetryBuilder openTelemetryBuilder
        , OtlpExportProtocol otlpExportProtocol = OtlpExportProtocol.Grpc
        , string otlpExportUri = "http://localhost:4317")
    {
        openTelemetryBuilder.UseOtlpExporter(otlpExportProtocol, new Uri(otlpExportUri));
    }
        


    // Do everything in one-method approach 👎👎

    public static void AddOpenTelemetry(IServiceCollection services, string serviceName, string serviceVersion
        , bool useConsoleExporterForTrace = false
        , bool useOtlpExporter = true
        , OpenTelemetry.Exporter.OtlpExportProtocol otlpExportProtocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
        , string otlpExportUri = "http://localhost:4317"
        , params string[] activitySourceNames)
    {
        var openTelemetryBuilder = services.AddOpenTelemetry();

        openTelemetryBuilder.ConfigureResource(builder =>
        {
            builder.AddService(serviceName: serviceName, serviceVersion: serviceVersion);
        });

        openTelemetryBuilder.WithTracing(builder =>
        {
            if (activitySourceNames.Length > 0)
                builder.AddSource(activitySourceNames);
            else
                builder.AddSource("*");

            if (useConsoleExporterForTrace) builder.AddConsoleExporter();
        });

        openTelemetryBuilder.UseOtlpExporter(otlpExportProtocol, new Uri(otlpExportUri));
    }

    // WORK-IN-PROGRESS
    // We want to based on configuration stored elsewhere (eg. appsettings.json), dumped in configuration
    // The above works, but has too many moving parts
    
    public static void AddOpenTelemetry(IConfiguration configuration, IServiceCollection services)
    {
        var openTelemetryBuilder = services.AddOpenTelemetry();

        openTelemetryBuilder.ConfigureResource(builder =>
        {
            builder.AddService(
                serviceName: "DemoUsingOpenApp10",
                serviceVersion: "1.0.0"
                );
            
        });

        openTelemetryBuilder.WithTracing(builder =>
        {
            //builder.SetResourceBuilder(
            //    ResourceBuilder.CreateDefault().AddService(
            //        serviceName: "DemoUsingOpenApp4",
            //        serviceVersion: "1.0.0"));
            //builder.AddSource("OpenTelemetry.Demo.Jaeger"); // Source here really means ActivitySource in .NET parlance
            builder.AddSource("*");

            // Add Exporters
            //builder.AddOtlpExporter(opt =>
            //{
            //    opt.Endpoint = new Uri("http://localhost:4317");
            //});
            //builder.AddConsoleExporter();
        });

        openTelemetryBuilder.UseOtlpExporter(OpenTelemetry.Exporter.OtlpExportProtocol.Grpc, new Uri("http://localhost:4317"));
    }

    //var otel = builder.Services.AddOpenTelemetry();
    //otel.ConfigureResource(builder =>
    //{
    //    //builder.AddService(
    //    //    serviceName: "DemoUsingOpenApp8",
    //    //    serviceVersion: "1.0.0"
    //    //    );
    //    builder.AddService(
    //        serviceName: "DemoUsingOpenApp10",
    //        serviceVersion: "1.0.0"
    //        );
    //    //ResourceBuilder.CreateDefault().AddService(serviceName);
    //});
    //otel.WithTracing(builder =>
    //{
    //    //builder.SetResourceBuilder(
    //    //    ResourceBuilder.CreateDefault().AddService(
    //    //        serviceName: "DemoUsingOpenApp4",
    //    //        serviceVersion: "1.0.0"));
    //    //builder.AddSource("OpenTelemetry.Demo.Jaeger"); // Source here really means ActivitySource in .NET parlance
    //    builder.AddSource("*");
    //    //builder.AddOtlpExporter(opt =>
    //    //{
    //    //    opt.Endpoint = new Uri("http://localhost:4317");
    //    //});
    //    //builder.AddConsoleExporter();
    //});
    ////otel.WithMetrics(builder =>
    ////{
    ////    builder.AddConsoleExporter();
    ////});
    //otel.UseOtlpExporter(OpenTelemetry.Exporter.OtlpExportProtocol.Grpc, new Uri("http://localhost:4317"));


}
