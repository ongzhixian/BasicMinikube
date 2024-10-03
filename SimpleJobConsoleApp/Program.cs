using Microsoft.Extensions.Configuration;

using Serilog;

IConfigurationRoot configurationRoot = GetConfiguration();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .ReadFrom.Configuration(configurationRoot)
    //.WriteTo.Console()
    //.WriteTo.File("logs/SimpleJobConsoleApp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Application {ApplicationStatus}", "START");

    Log.Information("USE_DUMMY_FLAG      {UseDummyFlag}"     , configurationRoot["FeatureFlags:USE_DUMMY_FLAG"]);
    Log.Information("TEST_EXCEPTION_FLAG {TestExceptionFlag}", configurationRoot["FeatureFlags:TEST_EXCEPTION_FLAG"]);
    

    bool testExceptionFlag = bool.TryParse(
        configurationRoot["FeatureFlags:TEST_EXCEPTION_FLAG"]
        , out bool parsedTestExceptionFlag)
        && parsedTestExceptionFlag;

    RunBrokenCode(testExceptionFlag);

    for (int i = 0; i < 9999; i++)
    {
        Console.WriteLine($"MessageE {i}");
        Thread.Sleep(1000);
    }

    Log.Information("Application {ApplicationStatus}", "END");
}
catch (Exception ex)
{
    Log.Error(ex, "Something went wrong");
}
finally
{
    await Log.CloseAndFlushAsync();
}


//try
//{
//    Console.WriteLine("Hello world");



//    //// Read secrets file
//    //var configyaml = File.ReadAllText("/opt/secret/config.yaml");
//    //Console.WriteLine("configyaml");
//    //Console.WriteLine(configyaml);

//    //var username = File.ReadAllText("/opt/secret/username");
//    //Console.WriteLine("username");
//    //Console.WriteLine(username);

//    //var password = File.ReadAllText("/opt/secret/password");
//    //Console.WriteLine("password");
//    //Console.WriteLine(password);

//    //// Read configmaps
//    //var appsettings = File.ReadAllText("/opt/configmap/appsettings.json");
//    //Console.WriteLine("appsettings.json in \"/opt/configmap/appsettings.json\"");
//    //Console.WriteLine(appsettings);


//    // Environment variables

//    //foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
//    //{
//    //    Console.WriteLine($"{entry.Key} = {entry.Value}");
//    //}

//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}


// PRIVATE METHODS

static IConfigurationRoot GetConfiguration()
{

    //Console.WriteLine($"Environment.CurrentDirectory is {Environment.CurrentDirectory}");
    //Console.WriteLine($"Directory.GetCurrentDirectory() is {Directory.GetCurrentDirectory()}");
    //Findings: They are the same for console app;

    var builder = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //A plain old console app does not have "env" (that comes from host builder)
        //Another way to check is use: https://stackoverflow.com/questions/1611410/how-to-check-if-a-app-is-in-debug-or-release
        //But that not strikes me particularly flexible either; 
        //KIV, until we find a better solution for simple console jobs
        //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

static void RunBrokenCode(bool testExceptionFlag)
{
    if (!testExceptionFlag) return;

    int a = 10, b = 0;

    Log.Debug("Dividing {A} by {B}", a, b);

    Console.WriteLine(a / b);
}