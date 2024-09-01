var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

AddDefaultRoute(app);
AddGetEnvironmentVariablesRoute(app);
AddGetDateRoute(app);


app.Run();

return;

void AddDefaultRoute(WebApplication app)
{
    app.MapGet("/", () =>
    {
        return "OK (SimpleWebApi)";
    })
    .WithName("GetDefault")
    .WithOpenApi();
}

void AddGetEnvironmentVariablesRoute(WebApplication app)
{
    app.MapGet("/environment-variables", () =>
    {
        foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            Console.WriteLine($"{entry.Key} = {entry.Value}");
        }
        return "OK (SimpleWebApi)";
    })
    .WithName("GetEnvironmentVariables")
    .WithOpenApi();

}

void AddGetDateRoute(WebApplication app)
{
    app.MapGet("/date", () =>
    {
        Console.WriteLine(DateTime.Now);
        return "OK (SimpleWebApi)";
    })
    .WithName("GetDate")
    .WithOpenApi();

}
