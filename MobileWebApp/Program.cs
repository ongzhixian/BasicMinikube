using Microsoft.AspNetCore.Authentication.Cookies;

using MobileWebApp.Repositories;
using MobileWebApp.Services;

using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAntiforgery(antiforgeryOptions =>
{
    antiforgeryOptions.Cookie.Name = "AppNonce";
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(authBuilder =>
    {
        authBuilder.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        authBuilder.SlidingExpiration = true;

        authBuilder.LoginPath = "/login";
        authBuilder.LogoutPath = "/logout";
        authBuilder.AccessDeniedPath = "/access-denied";

        authBuilder.Cookie.Name = "App";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
options.LoginPath = "/Login";
options.LogoutPath = "/Logout";
});

//builder.Services.AddScoped<IMongoClient>(sp =>
//    new MongoClient(builder.Configuration["ConnectionStrings:WareLogixMongoDb"])
//);
builder.Services.AddKeyedScoped<IMongoClient>("WareLogixMongoDb", (sp, key) =>
{
    return new MongoClient(builder.Configuration[$"ConnectionStrings:{key}"]);
});

builder.Services.AddKeyedScoped<IMongoDatabase>("minitools", (sp, key) =>
{
    var mongoClient = sp.GetRequiredKeyedService<IMongoClient>("WareLogixMongoDb");
    return mongoClient.GetDatabase((string)key);
});

builder.Services.AddScoped<AppUserRepository>();
builder.Services.AddScoped<AppRoleRepository>();

builder.Services.AddScoped<AppUserAuthenticationService>();
builder.Services.AddScoped<AppUserAuthorizationService>();
builder.Services.AddScoped<AppUserService>();
builder.Services.AddScoped<AppRoleService>();


builder.Services.AddHttpClient<EmailService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MailerSend:BaseAddressUri"] ?? "https://api.mailersend.com");
    var mailerSendApiToken = builder.Configuration["mailersend_api_token"] ?? throw new NullConfigurationException("mailersend_api_token");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mailerSendApiToken);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets()
   .RequireAuthorization();
app.MapControllers()
    .RequireAuthorization();

app.Run();
