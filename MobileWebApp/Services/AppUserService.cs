using System.Security.Claims;
using System.Security.Cryptography;

using MobileWebApp.Repositories;

namespace MobileWebApp.Services;

public class AppUserService
{
    private readonly ILogger<AppUserService> logger;
    private readonly AppUserRepository appUserRepository;

    public AppUserService(ILogger<AppUserService> logger, AppUserRepository appUserRepository)
    {
        this.logger = logger;
        this.appUserRepository = appUserRepository;
    }

    public async Task AddAppUserAsync(string username, string password)
    {
        var saltBytes = AppSecurityService.CreateSalt();

        var passwordBytes = password.ToUtf8Bytes();

        var sha256HashInBase64 = await GetSha256HashAsBase64Async(passwordBytes, saltBytes);

        await appUserRepository.AddUserAsync(
            username.ToUpperInvariant(),
            sha256HashInBase64,
            saltBytes.ToBase64()
        );

        //using var ms = new MemoryStream(passwordBytes.Length + salt.Length);
        //using var sha256= SHA256.Create();

        //ms.Write(passwordBytes);
        //ms.Write(salt);
        //ms.Position = 0;
    }

    public async Task<bool> CredentialsAreValidAsync(string username, string password)
    {
        var appUser = await appUserRepository.GetUserAsync(username);

        if (appUser == null) return false;

        return await SameHashAsync(password, appUser.PasswordSalt, appUser.PasswordHash);
    }

    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string username)
    {
        var appUser = await appUserRepository.GetUserAsync(username);

        if (appUser == null) return [];

        return appUser.Claims.Select(r => r);
    }


    internal async Task<long> GetUserCountAsync()
    {
        return await appUserRepository.GetUserCountAsync();
    }


    internal async Task<List<MongoDbModels.AppUser>> GetUserListAsync(int pageNumber, byte pageSize)
    {
        return await appUserRepository.GetUserList(pageNumber, pageSize);
    }

    // PRIVATE

    private async Task<bool> SameHashAsync(string password, string passwordSalt, string passwordHash)
    {
        var passwordBytes = password.ToUtf8Bytes();

        var saltBytes = passwordSalt.FromBase64();

        var sha256HashInBase64 = await GetSha256HashAsBase64Async(passwordBytes, saltBytes);

        return sha256HashInBase64.Equals(passwordHash);
    }

    private static async Task<string> GetSha256HashAsBase64Async(byte[] passwordBytes, byte[] saltBytes)
    {
        using var ms = new MemoryStream(passwordBytes.Length + saltBytes.Length);
        using var sha256 = SHA256.Create();

        ms.Write(passwordBytes);
        ms.Write(saltBytes);
        ms.Position = 0;

        return (await sha256.ComputeHashAsync(ms, CancellationToken.None)).ToBase64();
    }




    //public async Task GetAppUserAsync(string username)
    //{
    //    var appUser = await appUserRepository.GetUserAsync(username);
    //}

}