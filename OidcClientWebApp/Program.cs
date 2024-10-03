using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

string oidcServerUrl = "https://localhost:7120";

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(config =>
{
    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
 {
     // this is my Authorization Server Port
     options.Authority = oidcServerUrl;
     options.ClientId = "WareLogixOidcClientWebApp";
     options.ClientSecret = "123456789";
     options.ResponseType = "code";
     options.CallbackPath = "/signin-oidc";
     options.SaveTokens = true;
     //options.Scope.Add("jwtapitestapp.read");
     //options.GetClaimsFromUserInfoEndpoint = true;

     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = false,
         SignatureValidator = delegate (string token, TokenValidationParameters validationParameters)
         {
             //return new Microsoft.IdentityModel.Tokens.SecurityToken(token, validationParameters);
             //var jwt = new JwtSecurityToken(token);
             //return (Microsoft.IdentityModel.Tokens.SecurityToken)jwt;
             return new JsonWebToken(token);

         },
     };
 });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
