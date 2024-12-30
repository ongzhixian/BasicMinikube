using MobileWebApp.Repositories;

namespace MobileWebApp.Services;

public class BorrowService
{
    private readonly ILogger<BorrowService> logger;
    private readonly AppRoleRepository appRoleRepository;

    public BorrowService(ILogger<BorrowService> logger, AppRoleRepository appRoleRepository)
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

    internal async Task<IEnumerable<MongoDbModels.AppRole>> FindMatchingUserAsync(string searchCriteria)
    {
        return await appRoleRepository.FindMatchingUser(searchCriteria);
    }

    internal async Task<MongoDbModels.AppRole> GetRoleAsync(string roleName)
    {
        return await appRoleRepository.GetRoleAsync(roleName);
    }
}
