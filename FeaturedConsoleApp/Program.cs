
try
{
    // Read secrets file
    var configyaml = System.IO.File.ReadAllText("/opt/secret/config.yaml");
    Console.WriteLine("configyaml");
    Console.WriteLine(configyaml);

    var username = System.IO.File.ReadAllText("/opt/secret/username");
    Console.WriteLine("username");
    Console.WriteLine(username);

    var password = System.IO.File.ReadAllText("/opt/secret/password");
    Console.WriteLine("password");
    Console.WriteLine(password);

    // Read configmaps
    var appsettings = System.IO.File.ReadAllText("/opt/configmap/appsettings.json");
    Console.WriteLine("appsettings.json in \"/opt/configmap/appsettings.json\"");
    Console.WriteLine(appsettings);

    //System.Text.Json.JsonSerializer.Serialize(new Version(), );
    System.Text.Json.JsonSerializer.Deserialize("sd", )


    // Environment variables

    foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
    {
        Console.WriteLine($"{entry.Key} = {entry.Value}");
    }

    for (var i = 0; i < 1000; i++)
    {
        Thread.Sleep(3000);
        Console.WriteLine($"FeaturedConsoleApp announced at {DateTime.Now:O}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
