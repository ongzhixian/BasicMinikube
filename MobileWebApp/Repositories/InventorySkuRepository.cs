using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using MobileWebApp.MongoDbModels;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MobileWebApp.Repositories;

public class InventorySkuRepository
{
    private readonly IMongoCollection<InventorySku> inventorySkuCollection;

    public InventorySkuRepository(IConfiguration configuration,
        [FromKeyedServices("minitools")] IMongoDatabase database)
    {
        inventorySkuCollection = database.GetCollection<InventorySku>("inventory_sku");
    }

    internal async Task AddNewSkuAsync(string itemName, decimal quantity, string quantityUnit, string? skuId = null)
    {
        if (string.IsNullOrWhiteSpace(skuId))
            skuId = Guid.NewGuid().ToString();

        await CreateUniqueSkuIdIndexAsync();

        await inventorySkuCollection.InsertOneAsync(new InventorySku
        {
            SkuId = skuId,
            ItemName = itemName,
            Quantity = quantity,
            QuanityUnit = quantityUnit,
            Status = "Available"
        });
    }

    public async Task CreateUniqueSkuIdIndexAsync()
    {
        var options = new CreateIndexOptions { Name = "UniqueSkuIdIndex", Unique = true };

        var indexBuilder = Builders<InventorySku>.IndexKeys;
        var indexKeysDefinition = indexBuilder
            .Ascending(r => r.ItemName)
            .Ascending(r => r.SkuId);
        var createIndexModel = new CreateIndexModel<InventorySku>(indexKeysDefinition, options);

        // CreateOneIndexOptions createOneIndexOptions = new CreateOneIndexOptions();

        await inventorySkuCollection.Indexes.CreateOneAsync(createIndexModel);
    }

    internal async Task<long> GetInventorySkuCountAsync(string itemName)
    {
        var filter = Builders<InventorySku>.Filter.Eq(r => r.ItemName, itemName);

        return await inventorySkuCollection
            .Find(filter)
            .CountDocumentsAsync();
    }

    internal async Task<List<InventorySku>> GetInventorySkuListAsync(string itemName, int pageNumber, int pageSize)
    {
        var filter = Builders<InventorySku>.Filter.Eq(r => r.ItemName, itemName);

        var totalDocumentsCount = await inventorySkuCollection
            .Find(filter)
            .CountDocumentsAsync();
        
        var results = await inventorySkuCollection
            .Find(filter)
            .SortBy(r => r.SkuId)
            .Skip( (pageNumber - 1) * pageSize )
            .Limit(pageSize)
            .ToListAsync();
        
        return results;
    }



    //internal async Task<List<ItemSummary>> GetItemQuantitiesByItemNameAsync(List<InventoryItem> inventoryItems)
    //{
    //    var skuQuery = inventorySkuCollection.AsQueryable();

    //    var result = (
    //        from items in inventoryItems
    //        join skus in skuQuery.DefaultIfEmpty()
    //        on items.Name equals skus.ItemName
    //        group skus by skus.ItemName into skuItemGroup
    //        select new ItemSummary
    //        {
    //            ItemName = skuItemGroup.Key,
    //            Quantity = skuItemGroup.Sum(r => r.Quantity),
    //            QuantityUnit = skuItemGroup.FirstOrDefault().QuanityUnit ?? string.Empty,
    //        }).ToList();

    //    //var result = await skuQuery.GroupBy(r => r.ItemName)
    //    //    .OrderBy(r => r.Key)
    //    //    .Select(r => new ItemSummary
    //    //    {
    //    //        ItemName = r.Key,
    //    //        Quantity = r.Sum(r => r.Quantity),
    //    //        QuantityUnit = r.FirstOrDefault().QuanityUnit ?? string.Empty
    //    //    }).ToListAsync();

    //    return result;
    //}





    //internal async Task<List<InventoryItem>> GetAllInventoryItemsAsync()
    //{
    //    var filter = Builders<InventoryItem>.Filter.Empty;

    //    return await inventoryItemCollection
    //        .Find(filter)
    //        .SortBy(r => r.Name)
    //        .ToListAsync();
    //}

    //internal async Task<InventoryItem> GetInventoryItemAsync(string itemName)
    //{
    //    var filter = Builders<InventoryItem>.Filter.Eq(r => r.Name, itemName);

    //    return await inventoryItemCollection.Find(filter).FirstOrDefaultAsync();
    //}


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
