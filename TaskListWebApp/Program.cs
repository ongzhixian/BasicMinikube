using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using TaskListWebApp;
using TaskListWebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, DevAuthenticationStateProvider>();

//builder.Services.AddScoped<BlazorSchoolAuthenticationStateProvider>(); // 
//builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<BlazorSchoolAuthenticationStateProvider>());

//builder.Services.AddScoped<AuthenticationDataMemoryStorage>(); //
//builder.Services.AddScoped<BlazorSchoolUserService>(); // 

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


await builder.Build().RunAsync();
