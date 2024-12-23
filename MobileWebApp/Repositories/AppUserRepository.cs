using System.Runtime.CompilerServices;
using System.Security.Claims;

using MobileWebApp.MongoDbModels;

using MongoDB.Driver;

namespace MobileWebApp.Repositories;

public class AppUserRepository
{
    private readonly IMongoCollection<AppUser> appUserCollection;
    //private readonly IMongoDatabase database;

    public AppUserRepository(IConfiguration configuration,
        [FromKeyedServices("minitools")] IMongoDatabase database)
    {
        //var connectionUri = configuration["ConnectionStrings:WareLogixMongoDb"];
        //var client = new MongoClient(connectionUri);
        //IMongoDatabase database = client.GetDatabase("minitools");
        //IMongoCollection<AppUser> 
        appUserCollection = database.GetCollection<AppUser>("app_user");
    }


    public async Task AddUserAsync(string username, string hash, string salt)
    {
        await appUserCollection.InsertOneAsync(new AppUser
        {
            Username = username.ToUpperInvariant(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Applications = ["MOBILE APP"],
            Claims = [
                new Claim(ClaimTypes.Name, username.ToUpperInvariant())
            ]
        });
    }

    internal async Task<AppUser> GetUserAsync(string username)
    {
        var filter = Builders<AppUser>.Filter.Eq("Username", username);

        return await appUserCollection.Find(filter).FirstOrDefaultAsync();
    }

    internal async Task<long> GetUserCountAsync()
    {
        var filter = Builders<AppUser>.Filter.Empty;

        return await appUserCollection.CountDocumentsAsync(filter);
    }

    internal async Task<List<AppUser>> GetUserList(int pageNumber, byte pageSize = 10)
    {
        var filter = Builders<AppUser>.Filter.Empty;
        
        var recordsToSkip = (pageNumber - 1) * pageSize;

        return await appUserCollection
            .Find(filter)
            .SortBy(r => r.Username)
            .Skip(recordsToSkip)
            .Limit(pageSize)
            .ToListAsync();

        //var results = await appUserCollection.Find(filter).Skip(recordsToSkip).Limit(pageSize);
    }
}
