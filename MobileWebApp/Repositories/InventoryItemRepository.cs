






using MobileWebApp.MongoDbModels;

using MongoDB.Driver;

namespace MobileWebApp.Repositories;

public class InventoryItemRepository
{
    private readonly IMongoCollection<InventoryItem> inventoryItemCollection;

    public InventoryItemRepository(IConfiguration configuration,
        [FromKeyedServices("minitools")] IMongoDatabase database)
    {
        inventoryItemCollection = database.GetCollection<InventoryItem>("inventory_item");
    }

    internal async Task AddNewItem(string itemName)
    {
        await inventoryItemCollection.InsertOneAsync(new InventoryItem
        {
            Name = itemName,
            Quantity = 0,
            QuanityUnit = "PIECE",
            Status = "Available"
        });
    }

    public async Task CreateUniqueNameIndexAsync()
    {
        var options = new CreateIndexOptions { Name = "UniqueNameIndex", Unique = true };
        
        var indexBuilder = Builders<InventoryItem>.IndexKeys;
        var indexKeysDefinition = indexBuilder.Ascending(r => r.Name);
        var createIndexModel = new CreateIndexModel<InventoryItem>(indexKeysDefinition, options);

        // CreateOneIndexOptions createOneIndexOptions = new CreateOneIndexOptions();

        await inventoryItemCollection.Indexes.CreateOneAsync(createIndexModel);
    }

    internal async Task<List<InventoryItem>> GetAllInventoryItemsAsync(int pageNumber, int pageSize)
    {
        var filter = Builders<InventoryItem>.Filter.Empty;

        return await inventoryItemCollection
            .Find(filter)
            .SortBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    internal async Task<InventoryItem> GetInventoryItemAsync(string itemName)
    {
        var filter = Builders<InventoryItem>.Filter.Eq(r => r.Name, itemName);

        return await inventoryItemCollection.Find(filter).FirstOrDefaultAsync();
    }

    internal async Task UpdateAsync(InventoryItem inventoryItem)
    {
        var filter = Builders<InventoryItem>.Filter.Eq(r => r.Id, inventoryItem.Id);

        await inventoryItemCollection.ReplaceOneAsync(filter, inventoryItem);
    }

    internal async Task<long> GetAllInventoryItemCountAsync()
    {
        var filter = Builders<InventoryItem>.Filter.Empty;

        return await inventoryItemCollection.CountDocumentsAsync(filter);
    }




    //public async Task AddRoleAsync(string roleName)
    //{
    //    await inventoryItemCollection.InsertOneAsync(new AppRole
    //    {
    //        RoleName = roleName
    //    });
    //}

    //public async Task<IEnumerable<AppRole>> GetAllRolesAsync()
    //{
    //    var filter = Builders<AppRole>.Filter.Empty;

    //    return await inventoryItemCollection.Find(filter).ToListAsync();
    //}

    //public async Task<long> GetRolesCountAsync()
    //{
    //    var filter = Builders<AppRole>.Filter.Empty;

    //    return await inventoryItemCollection.CountDocumentsAsync(filter);
    //}

    //internal async Task<IEnumerable<MongoDbModels.AppRole>> FindMatchingUser(string searchCriteria)
    //{
    //    try
    //    {
    //        var filter = Builders<AppRole>.Filter.Empty;

    //        if (!searchCriteria.Equals("*"))
    //            filter = Builders<AppRole>.Filter.Regex(r => r.RoleName,
    //                new MongoDB.Bson.BsonRegularExpression(searchCriteria, "i"));

    //        var results = await inventoryItemCollection
    //            .Find(filter)
    //            .SortBy(r => r.RoleName)
    //            .ToListAsync();

    //        return results;
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}

    //internal async Task<AppRole> GetRoleAsync(string roleName)
    //{
    //    var filter = Builders<AppRole>.Filter.Eq("RoleName", roleName);

    //    return await inventoryItemCollection.Find(filter).FirstOrDefaultAsync();
    //}
}
