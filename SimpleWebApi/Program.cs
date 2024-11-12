using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using SimpleWebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        //builder.AllowAnyOrigin()
        //       .AllowAnyHeader()
        //       .AllowAnyMethod();

        builder.WithOrigins("https://localhost:5207", "http://localhost:5206")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();

    });

    //options.AddPolicy("mypolicy", builder => builder
    //    .WithOrigins("http://localhost:4200/")
    //    .SetIsOriginAllowed((host) => true)
    //    .AllowAnyMethod()
    //    .AllowAnyHeader());

    //app.UseCors(x => x
    //    .AllowAnyMethod()
    //    .AllowAnyHeader()
    //    .SetIsOriginAllowed(origin => true) // allow any origin
    //    .AllowCredentials()); // allow credentials

    //options.AddPolicy("CorsPolicy",
    //        builder => builder.AllowAnyOrigin()
    //        .AllowAnyMethod()
    //        .AllowAnyHeader()
    //        .AllowCredentials());
});

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger(c =>
    //{
    //    c.PreSerializeFilters.Add((swagger, httpReq) =>
    //    {
    //        swagger.Servers = new List<OpenApiServer> { 
    //            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
    //            new OpenApiServer { Url = $"https://localhost:5207" },
    //        };
    //    });
    //});
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();

AddDefaultRoute(app);
AddGetEnvironmentVariablesRoute(app);
AddGetDateRoute(app);

app.MapHub<ChatHub>("/chatHub");

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

    app.MapGet("/200", ([Microsoft.AspNetCore.Mvc.FromQuery] ushort sleep = ushort.MinValue) =>
    {
        if (sleep > 0) 
            Thread.Sleep(sleep);

        return "OK (SimpleWebApi)";
    })
    .WithName("GetHttp200")
    .WithOpenApi();

    app.MapGet("/204", ([Microsoft.AspNetCore.Mvc.FromQuery] ushort sleep = ushort.MinValue) =>
    {
        if (sleep > 0)
            Thread.Sleep(sleep);

        return Microsoft.AspNetCore.Http.Results.NoContent();
    })
    .WithName("GetHttp204")
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
