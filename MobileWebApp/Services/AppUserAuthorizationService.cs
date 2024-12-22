using System.Security.Claims;

namespace MobileWebApp.Services;

public class AppUserAuthorizationService
{
    private readonly ILogger<AppUserAuthorizationService> logger;
    private readonly AppUserService appUserService;
    
    public AppUserAuthorizationService(ILogger<AppUserAuthorizationService> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(string username)
    {
        logger.LogInformation("Retrieve {Username} security claims", username);
        return await appUserService.GetUserClaimsAsync(username);
    }
}
