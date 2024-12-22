namespace MobileWebApp.Services;

public class AppUserAuthenticationService
{
    private readonly ILogger<AppUserAuthenticationService> logger;
    private readonly AppUserService appUserService;
    
    
    public AppUserAuthenticationService(ILogger<AppUserAuthenticationService> logger, AppUserService appUserService)
    {
        this.logger = logger;
        this.appUserService = appUserService;
    }

    public async Task<bool> CredentialsAreValidAsync(string username, string password)
    {
        logger.LogInformation("");
        return await appUserService.CredentialsAreValidAsync(username, password);
    }
}
