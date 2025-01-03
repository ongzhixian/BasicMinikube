﻿
using MobileWebApp.MongoDbModels;

using MongoDB.Driver;

namespace MobileWebApp.Repositories;

public class AppRoleRepository
{
    private readonly IMongoCollection<AppRole> appRoleCollection;

    public AppRoleRepository(IConfiguration configuration,
        [FromKeyedServices("minitools")] IMongoDatabase database)
    {
        appRoleCollection = database.GetCollection<AppRole>("app_role");
    }


    public async Task AddRoleAsync(string roleName)
    {
        await appRoleCollection.InsertOneAsync(new AppRole
        {
            RoleName = roleName
        });
    }

    public async Task<IEnumerable<AppRole>> GetAllRolesAsync()
    {
        var filter = Builders<AppRole>.Filter.Empty;

        return await appRoleCollection.Find(filter).ToListAsync();
    }

    public async Task<long> GetRolesCountAsync()
    {
        var filter = Builders<AppRole>.Filter.Empty;

        return await appRoleCollection.CountDocumentsAsync(filter);
    }

    internal async Task<IEnumerable<MongoDbModels.AppRole>> FindMatchingUser(string searchCriteria)
    {
        try
        {
            var filter = Builders<AppRole>.Filter.Empty;

            if (!searchCriteria.Equals("*"))
                filter = Builders<AppRole>.Filter.Regex(r => r.RoleName,
                    new MongoDB.Bson.BsonRegularExpression(searchCriteria, "i"));

            var results = await appRoleCollection
                .Find(filter)
                .SortBy(r => r.RoleName)
                .ToListAsync();

            return results;
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal async Task<AppRole> GetRoleAsync(string roleName)
    {
        var filter = Builders<AppRole>.Filter.Eq("RoleName", roleName);

        return await appRoleCollection.Find(filter).FirstOrDefaultAsync();
    }
}
