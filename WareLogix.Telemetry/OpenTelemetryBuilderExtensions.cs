using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WareLogix.Telemetry;

public static class OpenTelemetryBuilderExtensions
{
    public static OpenTelemetryBuilder DefineResourceService(this OpenTelemetryBuilder openTelemetryBuilder, string serviceName, string serviceVersion)
    {
        openTelemetryBuilder.ConfigureResource(builder =>
        {
            builder.AddService(serviceName: serviceName, serviceVersion: serviceVersion);
        });

        return openTelemetryBuilder;
    }


    public static OpenTelemetryBuilder AddTracing(this OpenTelemetryBuilder openTelemetryBuilder, bool useConsoleExporterForTrace = false, params string[] activitySourceNames)
    {
        openTelemetryBuilder.WithTracing(builder =>
        {
            if (activitySourceNames.Length > 0)
                builder.AddSource(activitySourceNames);
            else
                builder.AddSource("*");

            if (useConsoleExporterForTrace) builder.AddConsoleExporter();
        });

        return openTelemetryBuilder;
    }

    public static OpenTelemetryBuilder AddTracing(this OpenTelemetryBuilder openTelemetryBuilder, bool useConsoleExporterForTrace = false) =>
        AddTracing(openTelemetryBuilder, useConsoleExporterForTrace, activitySourceNames: "*");

}
