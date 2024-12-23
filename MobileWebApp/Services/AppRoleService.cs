
using MobileWebApp.Repositories;

namespace MobileWebApp.Services;

public class AppRoleService
{
    private readonly ILogger<AppRoleService> logger;
    private readonly AppRoleRepository appRoleRepository;

    public AppRoleService(ILogger<AppRoleService> logger, AppRoleRepository appRoleRepository)
    {
        this.logger = logger;
        this.appRoleRepository = appRoleRepository;
    }

    public async Task AddRoleAsync(string roleName)
    {
        await appRoleRepository.AddRoleAsync(roleName);
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        return (await appRoleRepository.GetAllRolesAsync()).Select(x => x.RoleName);
    }

    public async Task<long> GetAllRolesCountAsync()
    {
        return await appRoleRepository.GetRolesCountAsync();
            
    }
}
