using System.Security.Principal;

using MobileWebApp.MongoDbModels;

namespace MobileWebApp.Services;

public class SessionService
{
    private readonly ISession? session;
    private readonly IIdentity? user;
    private readonly AppUserService appUserService;

    public SessionService(IHttpContextAccessor httpContextAccessor, AppUserService appUserService)
    {
        session = httpContextAccessor.HttpContext?.Session;
        user = httpContextAccessor.HttpContext?.User.Identity;
        this.appUserService = appUserService;
    }


    public async Task LoadUserPreferencesToSessionAsync(string username)
    {
        if (session == null) return;

        var appUser = await appUserService.GetUserAsync(username);

        session.SetInt32(UserPreferences.PREFERRED_PAGE_SIZE, appUser.UserPreferences.PreferredPageSize);
    }


    public async Task<int?> GetInt32Async(string key)
    {
        if (session == null) return null;
        if (user == null) return null;

        if (!session.Keys.Any() && user.IsAuthenticated && user.Name is { })
            await LoadUserPreferencesToSessionAsync(user.Name);

        return session.GetInt32(key);
    }

}
