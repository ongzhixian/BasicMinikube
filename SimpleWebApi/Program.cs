using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using SimpleWebApi;
using SimpleWebApi.Authentication;
using SimpleWebApi.Extensions;
using SimpleWebApi.Hubs;
using SimpleWebApi.RouteGroups;
using SimpleWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Suppress "Server" headers in response ; not sure if can set in appsettings.json instead
    options.AddServerHeader = false;
    options.Limits.MaxRequestBufferSize = null;
    options.Limits.MinRequestBodyDataRate = null;
    options.Limits.MinResponseDataRate = null;

});

builder.Services.AddHttpClient();
builder.Services.AddHttpLogging(o => { });

//builder.Services.AddProblemDetails();

const string serviceName = "SimpleWebApi";

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddConsoleExporter();
});

//builder.Services.Configure<OpenTelemetry.Exporter.OtlpExporterOptions>(o => {
//});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(opt =>
          {
              
              //opt.ExportProcessorType = OpenTelemetry.ExportProcessorType.
              opt.Protocol = OtlpExportProtocol.HttpProtobuf;

              //opt.Endpoint = new Uri("http://localhost:4317");
              //opt.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
              opt.Endpoint = new Uri("https://tempo-us-central1.grafana.net");

              string username = "someusername";
              string password = "somepassword";
              
              //var authenticationString = $"{username}:{password}";
              //var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

              
              opt.Protocol = OtlpExportProtocol.HttpProtobuf;
              opt.HttpClientFactory = () =>
              {
                  HttpClient client = new HttpClient();
                  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                      "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                          $"{username}:{password}")));

                  //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                  //    "Basic", $"{username}:{password}");

                  //client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value");

                  Console.WriteLine("Created OLTP client");


                  return client;
              };



          })
      //).WithMetrics(metrics => metrics
      //    .AddAspNetCoreInstrumentation()
      //    .AddConsoleExporter()
      //    .AddOtlpExporter(opt =>
      //    {
      //        opt.Endpoint = new Uri("http://localhost:4317");
      //    })
          );

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    SymmetricSecurityKey issuerSigningKey = new(
        Encoding.UTF8.GetBytes(
            builder.Configuration["DailyWorkJournalSecurityKey"] ?? throw new NullReferenceException("DailyWorkJournalSecurityKey configuration missing")));

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = false,

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:Issuer"],

        ValidateAudience = true,
        ValidAudiences = builder.Configuration.GetSection("Authentication:Schemes:Bearer:Audiences").Get<IEnumerable<string>>(),
        //ValidAudience = builder.Configuration["Authentication:Schemes:Bearer:Audiences"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = issuerSigningKey,
    };

}).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthentication.AuthenticationScheme, null);
;



MongoDbServices.AddDefinitions(builder.Services, builder.Configuration);

builder.Services.AddScoped<CourseService>();

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(op =>
{
    op.OperationFilter<AuthResponsesOperationFilter>();
    //op.OperationFilter<SecurityRequirementsOperationFilter>();
});

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

app.UseHttpLogging();

//app.UseAuthentication();

//app.UseAuthorization();

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

            //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //{
            //    Name = "Authorization",
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer",
            //    BearerFormat = "JWT",
            //    In = ParameterLocation.Header,
            //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            //});

            //swagger.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme()
            //{
            //    Name = "Authorization",
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer",
            //    BearerFormat = "JWT",
            //    In = ParameterLocation.Header,
            //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            //});

            // This add security requirements for all operations
            // But ideally, we want to skip adding security requirements (the 'padlock') if its an anonymous operation
            //swagger.SecurityRequirements.Add(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {

            //            Reference = new OpenApiReference
            //            {
            //                Type=ReferenceType.SecurityScheme,
            //                Id="Bearer"
            //            }
            //        },
            //        []
            //    }
            //});


            // TODO: Further investigare what can be customized with "swagger"
        });

    });

    app.UseSwaggerUI(swaggerUiOptions =>
    {
        swaggerUiOptions.DocumentTitle = "Eima Open API";
        //swaggerUiOptions.RoutePrefix = "swagger";
        //swaggerUiOptions.SwaggerEndpoint("v1/swagger.json", "My API V1");


    });
}

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseCorrelationIdMiddleware();

AddWeatherForecastApiRoute(app);
AddBasicAuthorizeTestApiRoute(app);
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


app.MapGroup("/health")
    .WithHealthApi()
    //.RequireAuthorization();
    //.AllowAnonymous()
    ;

app.MapGroup("/public/course")
    .WithCourseApi();

app.MapGroup("/public/json-web-token")
    .WithJsonWebTokenApi();


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


void AddWeatherForecastApiRoute(WebApplication app)
{
    app.MapGet("/weather-forecast", async (HttpContext ctx, HttpClient httpClient, IHttpClientFactory fact) =>
    {
        //var webProxy = new WebProxy
        //{
        //    Address = new Uri($"http://localhost:8083"),
        //    BypassProxyOnLocal = false,
        //    UseDefaultCredentials = false,
        //};

        //HttpClientHandler httpClientHandler = new HttpClientHandler
        //{
        //    Proxy = webProxy
        //};

        //httpClient = new HttpClient(httpClientHandler);

        var results = await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>(new Uri("http://localhost:5230/weatherforecast"),
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

        return results;
    })
    //.Accepts<string>("plain/text")
    .Produces<string>(StatusCodes.Status200OK)
    .WithOpenApi(operation =>
    {
        operation.OperationId = "GetWeatherForecast";
        operation.Summary = "Dummy weather forecast";
        operation.Description = "Dummy weather forecast";
        operation.Deprecated = false;
        operation.Tags = new List<OpenApiTag> { new() { Name = "Weather Forecast" } };

        

        return operation;
    });

}



void AddBasicAuthorizeTestApiRoute(WebApplication app)
{
    app.MapGet("/basic-auth-test", [BasicAuthorizationAttribute] async () =>
    {
        return TypedResults.Ok("OKOK");
    })
    .Produces<string>(StatusCodes.Status200OK)
    .WithOpenApi(operation =>
    {
        operation.OperationId = "TestBasicAuthorize";
        operation.Summary = "Get some info using endpoint that requires basic authorization";
        operation.Description = "Get some info using endpoint that requires basic authorization";
        operation.Deprecated = false;
        operation.Tags = new List<OpenApiTag> { new() { Name = "Test Authorization",  } };



        return operation;
    });

}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

