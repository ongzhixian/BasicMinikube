//using System.Security.Claims;

//using Microsoft.AspNetCore.Components.Authorization;

//using TaskListWebApp.Models;

//namespace TaskListWebApp.Services;


//public class BlazorSchoolAuthenticationStateProvider : AuthenticationStateProvider
//{
//    private readonly BlazorSchoolUserService _blazorSchoolUserService;

//    public User CurrentUser { get; private set; } = new();

//    public BlazorSchoolAuthenticationStateProvider(BlazorSchoolUserService blazorSchoolUserService)
//    {
//        _blazorSchoolUserService = blazorSchoolUserService;
//    }

//    public async Task LoginAsync(string username, string password)
//    {
//        var principal = new ClaimsPrincipal();
//        var user = await _blazorSchoolUserService.SendAuthenticateRequestAsync(username, password);

//        if (user is not null)
//        {
//            principal = user.ToClaimsPrincipal();
//        }

//        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
//    }

//    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//    {
//        var principal = new ClaimsPrincipal();
//        var user = _blazorSchoolUserService.FetchUserFromBrowser();

//        if (user is not null)
//        {
//            var authenticatedUser = await _blazorSchoolUserService.SendAuthenticateRequestAsync(user.Username, user.Password);

//            if (authenticatedUser is not null)
//            {
//                principal = authenticatedUser.ToClaimsPrincipal();
//            }
//        }

//        return new(principal);
//    }

//    public void Logout()
//    {
//        _blazorSchoolUserService.ClearBrowserUserData();
//        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
//    }

//    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
//    {
//        var authenticationState = await task;

//        if (authenticationState is not null)
//        {
//            CurrentUser = User.FromClaimsPrincipal(authenticationState.User);
//        }
//    }

//    public BlazorSchoolAuthenticationStateProvider(BlazorSchoolUserService blazorSchoolUserService)
//    {
//        _blazorSchoolUserService = blazorSchoolUserService;
//        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
//    }

//    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;

//}