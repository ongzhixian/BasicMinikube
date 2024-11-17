using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;

using SimpleWebApi.Extensions;
using SimpleWebApi.Hubs;
using SimpleWebApi.RouteGroups;
using SimpleWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Suppress "Server" headers in response ; not sure if can set in appsettings.json instead
    options.AddServerHeader = false; 
});

//builder.Services.AddProblemDetails();

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



MongoDbServices.AddDefinitions(builder.Services, builder.Configuration);

builder.Services.AddScoped<CourseService>();

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-9.0#exception-handler-lambda
// See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-9.0#exception-handler-page

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        if (context.Features.Get<IExceptionHandlerFeature>() is { } exceptionHandlerFeature)
        {
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
            context.Response.StatusCode = exceptionHandlerFeature.Error switch
            {
                Microsoft.AspNetCore.Http.BadHttpRequestException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };
            
            if (exceptionHandlerFeature.Error.InnerException != null)
                await context.Response.WriteAsync($"{exceptionHandlerFeature.Error.Message}\r\n{exceptionHandlerFeature.Error.InnerException.Message}");
            else
                await context.Response.WriteAsync($"{exceptionHandlerFeature.Error.Message}");
                
            return;
        }

        //app.UseExceptionHandler(exceptionHandlerApp => {exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));
        await Results.Problem().ExecuteAsync(context);
    });
});


app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(swaggerOptions =>
    {
        swaggerOptions.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Info.Title = "Eima API";
            swagger.Info.Version = "v1";
            swagger.Info.Description = "API for Education Institution Management Application (Eima for short)";
            // Eima => acronym for Education Institution Management Application
            swagger.Info.License = new OpenApiLicense { Name = "Apache License 2.0", Url = new Uri($"{httpReq.Scheme}://{httpReq.Host.Value}/license.html") };
            swagger.Info.TermsOfService = new Uri($"{httpReq.Scheme}://{httpReq.Host.Value}/terms-of-service.html");
            swagger.Info.Contact = new OpenApiContact { Name = "Contact person", Email = "zhixian@hotmail.com", Url = new Uri($"{httpReq.Scheme}://{httpReq.Host.Value}/contact.html") };

            swagger.Servers = new List<OpenApiServer> {
                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
                new OpenApiServer { Url = $"https://localhost:5207" },
            };

            // TODO: Further investigare what can be customized with "swagger"
        });
    });

    app.UseSwaggerUI(swaggerUiOptions =>
    {
        swaggerUiOptions.DocumentTitle = "Eima Open API";
    });
}

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseCorrelationIdMiddleware();

//AddDefaultRoute(app);
//AddGetEnvironmentVariablesRoute(app);
//AddGetDateRoute(app);
//StudentApi.AddStudentApi(app);

//app.MapGet("/terms-of-service", () =>
//{
//    return Results.Content("<h1>Thanks for accepting our terms of service!</h1>", "text/html");
//});
//app.MapGet("/contact", () =>
//{
//    return Results.Content("<h1>Contact information to be added here!</h1>", "text/html");
//});

app.MapGroup("/public/course")
    .WithCourseApi();


app.MapHub<ChatHub>("/chatHub");

//app.UseCors("AllowAnyPolicy");



app.Run();

return;

////////////////////////////////////////
// ROUTES 

void AddDefaultRoute(WebApplication app)
{
    app.MapGet("/", () =>
    {
        //return TypeResults"OK (SimpleWebApi)";
        return TypedResults.Ok("OK");
    })
    .Accepts<string>("plain/text")
    .Produces<string>(StatusCodes.Status200OK)
    //.Produces(StatusCodes.Status404NotFound)
    //.WithDisplayName("Default endpoint")
    //.WithTags("TodoGroup")
    //.WithName("GetDefault") // Operation id
    .WithOpenApi(operation => 
    {
        operation.OperationId = "GetTodos";
        operation.Summary = "This is a summary";
        operation.Description = "This is a description";
        operation.Deprecated = true;
        operation.Tags = new List<OpenApiTag> { new() { Name = "Todos" } };


        
        //var parameter = operation.Parameters[0];
        //parameter.Description = "The ID associated with the created Todo";

        return operation;
    });

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
    //.WithOpenApi(operation =>
    //{

    ////     //operation.OperationId = "CourseOps";
    ////     //operation.Summary = "This is a summary";
    ////     //operation.Description = "This is a description";

    ////     //operation.Deprecated = true;
    ////     operation.Tags = new List<OpenApiTag> { new() { Name = "Course" } };
    ////     //var parameter = operation.Parameters[0];
    ////     //parameter.Description = "The ID associated with the created Todo";

    //    return operation;
    //});
}
