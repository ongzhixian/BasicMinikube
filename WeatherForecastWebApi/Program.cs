using Serilog;
using Serilog.Events;

using WareLogix.WebApi.Extensions;
using WareLogix.WebApi.Metrics;

using static System.Net.WebRequestMethods;

//Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
//Serilog.Debugging.SelfLog.Enable(Console.Error);
//Serilog.Debugging.SelfLog.WriteLine("APPLICATION STARTED {0}", DateTime.UtcNow);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

    builder.Host.UseSerilog((context, serviceProvider) => serviceProvider
        .ReadFrom.Configuration(context.Configuration)
        //.ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions { SectionName = "SerilogTraceLog" }), preserveStaticLogger: IsRunningInTestHostProcess()
        );

    // Add services to the container.

    builder.Services.AddMetrics();

    // TODO: Add health checks
    // https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0

    builder.Services.AddSingleton<RequestMetric>();
    //builder.Services.AddSingleton<RequestMetric>(sp => new RequestMetric("meter2", sp.GetRequiredService<IMeterFactory>()));
    //builder.Services.AddSingleton<RequestMetric>(sp => new RequestMetric("meter3", sp.GetRequiredService<IMeterFactory>()));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // TODO: Add feature flag
    //app.UseHttpsRedirection();

    app.UseCorrelationIdMiddleware();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Uncaught exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
