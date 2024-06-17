using System.IO;
using System;

using Serilog;
using Serilog.Events;

using WareLogix.WebApi.Extensions;
using WareLogix.WebApi.Metrics;

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

    

    if (bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out bool parsedDotnetRunningInContainer) && parsedDotnetRunningInContainer)
    {
        Log.Information("USING SECRETS FILE");

        // The 'AddJsonFile' does not throw an error; it just fails silently :-(
        // As a result the dotfile (.appsettings.secrets.json in this case) is not read/loaded
        // I suspect this is because dotfiles are readonly under Linux filesystems
        // builder.Configuration.AddJsonFile("./.appsettings.secrets.json", true, true);

        // We need to specify FileAccess.Read, otherwise we get the following exception:
        // System.IO.IOException: Read-only file system : '/opt/app/.appsettings.secrets.json'
        // at Interop.ThrowExceptionForIoErrno(ErrorInfo errorInfo, String path, Boolean isDirectory, Func`2 errorRewriter)

        using FileStream fs = new("./.appsettings.secrets.json", FileMode.Open, FileAccess.Read);
        builder.Configuration.AddJsonStream(fs);


        //var secrets = sr.ReadToEnd();
        //Log.Information(".appsettings.secrets.json contains {Secrets}", secrets);

        //builder.Configuration.AddJsonFile("./.appsettings.secrets.json", true, true);

    }
    Log.Information("parsedDotnetRunningInContainer is {ParsedDotnetRunningInContainer}", parsedDotnetRunningInContainer);

    Log.Information(builder.Configuration.GetDebugView());

    //var syn1 = System.IO.File.Exists("./.appsettings.secrets.json"); // T
    //var syn2 = System.IO.File.Exists("/.appsettings.secrets.json");  // F
    //var syn3 = System.IO.File.Exists(".appsettings.secrets.json");   // T

    //Log.Information("Syn1 {Syn1} ; Syn2 {Syn2} ; Syn3 {Syn3}", syn1, syn2, syn3);

    

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
