using System.Reflection;

var assemblyLocation = Assembly.GetExecutingAssembly().Location;
var assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? throw new Exception($"No directory name for {assemblyLocation}");

Console.WriteLine("[PROGRAM START]");
Console.WriteLine($"Runtime directory is [{Environment.CurrentDirectory}]");
Console.WriteLine($"  Executable path is [{assemblyLocation}]");
Console.WriteLine($"  Executable dir  is [{assemblyDirectory}]");

var appsettingsJsonPath = Path.Combine(assemblyDirectory, "appsettings.json");
if (File.Exists(appsettingsJsonPath))
{
    using StreamReader reader = new(appsettingsJsonPath);
    string contents = reader.ReadToEnd();
    Console.WriteLine(contents);
}

for (var i = 0; i < 10; i++)
{
    Thread.Sleep(3000);
    Console.WriteLine($"Hello, World announced at {DateTime.Now:O}");
}

Console.WriteLine("[PROGRAM END]");