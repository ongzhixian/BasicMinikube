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
}
