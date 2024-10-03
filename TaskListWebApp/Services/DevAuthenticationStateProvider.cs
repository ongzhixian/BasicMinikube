using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace TaskListWebApp.Services;

public class DevAuthenticationStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(1500);

        var anonymous = new ClaimsIdentity();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        anonymous = new ClaimsIdentity(claims, "testAuthType");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
    }
}
