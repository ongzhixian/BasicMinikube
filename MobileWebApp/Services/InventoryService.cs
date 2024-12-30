using MobileWebApp.Models;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Repositories;

using MongoDB.Driver;

using static MobileWebApp.Repositories.InventorySkuRepository;

namespace MobileWebApp.Services;

public class InventoryService
{
    private readonly ILogger<InventoryService> logger;
    private readonly InventoryItemRepository inventoryItemRepository;
    private readonly InventorySkuRepository inventorySkuRepository;
    private readonly IMongoClient mongoClient;

    public InventoryService(ILogger<InventoryService> logger,
        [FromKeyedServices("WareLogixMongoDb")]IMongoClient mongoClient,
        InventoryItemRepository inventoryItemRepository, InventorySkuRepository inventorySkuRepository)
    {
        this.logger = logger;
        this.mongoClient = mongoClient;
        this.inventoryItemRepository = inventoryItemRepository;
        this.inventorySkuRepository = inventorySkuRepository;
    }

    internal async Task AddNewItemAsync(string itemName)
    {
        await inventoryItemRepository.AddNewItem(itemName);
    }

    internal async Task<PageOf<InventoryItem>> GetAllInventoryItemsAsync(int pageNumber, int pageSize)
    {
        //return await inventoryItemRepository.GetAllInventoryItemsAsync(pageNumber, pageSize);

        var documentCount = await inventoryItemRepository.GetAllInventoryItemCountAsync();
        var results = await inventoryItemRepository.GetAllInventoryItemsAsync(pageNumber, pageSize); ;

        return new PageOf<InventoryItem>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = documentCount,
            Data = results
        };
    }

    internal async Task<InventoryItem> GetInventoryItemAsync(string itemName)
    {
        return await inventoryItemRepository.GetInventoryItemAsync(itemName);
    }

    //internal async Task<List<InventoryItem>> GetInventoryItemQuantitiesAsync()
    //{
    //    var inventoryItems = await inventoryItemRepository.GetAllInventoryItemsAsync();

    //    return inventoryItems;
    //    //return await inventorySkuRepository.GetItemQuantitiesByItemNameAsync(inventoryItems);
    //}

    internal async Task<PageOf<InventorySku>> GetSkusByItemNameAsync(string itemName, int pageNumber, int pageSize)
    {
        var documentCount = await inventorySkuRepository.GetInventorySkuCountAsync(itemName);
        var results = await inventorySkuRepository.GetInventorySkuListAsync(itemName, pageNumber, pageSize);

        return new PageOf<InventorySku>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = documentCount,
            Data = results
        };
    }

    internal async Task IncreaseItemQuantityAsync(string itemName, decimal itemQuantity)
    {
        

        using (var mongodbSession = await mongoClient.StartSessionAsync())
        {
            mongodbSession.StartTransaction();

            var inventoryItem = await inventoryItemRepository.GetInventoryItemAsync(itemName);

            try
            {
                if (int.TryParse(itemQuantity.ToString(), out int quantity))
                {
                    for (int i = 0; i < itemQuantity; i++)
                    {
                        await inventorySkuRepository.AddNewSkuAsync(inventoryItem.Name, 1, inventoryItem.QuanityUnit);
                    }
                }

                inventoryItem.Quantity += itemQuantity;

                await inventoryItemRepository.UpdateAsync(inventoryItem);

                await mongodbSession.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await mongodbSession.AbortTransactionAsync();
            }

        }

            

        //return await inventoryItemRepository.IncreaseItemQuantityAsync(itemName, itemQuantity);
    }
}
