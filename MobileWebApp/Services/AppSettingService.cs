namespace MobileWebApp.Services;

public class AppSettingService
{
    public readonly int DEFAULT_PAGE_SIZE;

    public AppSettingService(IConfiguration configuration)
    {
        if (!int.TryParse(configuration["Application:DefaultPageSize"], out DEFAULT_PAGE_SIZE))
            DEFAULT_PAGE_SIZE = 10;
            
    }

}
