using MobileWebApp.Models;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Repositories;

using MongoDB.Driver;

namespace MobileWebApp.Services;

public class InventoryService
{
    private readonly ILogger<InventoryService> logger;
    private readonly InventoryItemRepository inventoryItemRepository;
    private readonly InventorySkuRepository inventorySkuRepository;
    private readonly IMongoClient mongoClient;

    public InventoryService(ILogger<InventoryService> logger,
        [FromKeyedServices("WareLogixMongoDb")] IMongoClient mongoClient,
        InventoryItemRepository inventoryItemRepository, InventorySkuRepository inventorySkuRepository)
    {
        this.logger = logger;
        this.mongoClient = mongoClient;
        this.inventoryItemRepository = inventoryItemRepository;
        this.inventorySkuRepository = inventorySkuRepository;
    }

    // INVENTORY ITEM

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
    
    internal async Task<OperationResult> RemoveItemAsync(string itemName)
    {
        const string operationName = "Remove Inventory Item";

        var documentCount = await inventorySkuRepository.GetInventorySkuCountAsync(itemName);

        if (documentCount > 0) return new FailureResult(operationName, "SKUs exists for inventory item");

        if (await inventoryItemRepository.RemoveInventoryItem(itemName) > 0)
            return new SuccessResult(operationName, "Inventory item removed");
        
        return new FailureResult(operationName, "Inventory item not removed");

        //var results = await inventorySkuRepository.GetInventorySkuListAsync(itemName, pageNumber, pageSize);
    }

    //internal async Task<List<InventoryItem>> GetInventoryItemQuantitiesAsync()
    //{
    //    var inventoryItems = await inventoryItemRepository.GetAllInventoryItemsAsync();

    //    return inventoryItems;
    //    //return await inventorySkuRepository.GetItemQuantitiesByItemNameAsync(inventoryItems);
    //}

    // INVENTORY SKUs

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

    internal async Task<OperationResult> AddInventorySkuAsync(string itemName, string skuId)
    {
        const string operationName = "Add Inventory SKU";

        using (var mongodbSession = await mongoClient.StartSessionAsync())
        {
            mongodbSession.StartTransaction();

            var inventoryItem = await inventoryItemRepository.GetInventoryItemAsync(itemName);

            try
            {
                await inventorySkuRepository.AddNewSkuAsync(inventoryItem.Name, 1, inventoryItem.QuanityUnit, skuId);

                inventoryItem.Quantity += 1;

                await inventoryItemRepository.UpdateAsync(inventoryItem);

                await mongodbSession.CommitTransactionAsync();

                return new SuccessResult(operationName, "Inventory SKU added");
            }
            catch (Exception)
            {
                await mongodbSession.AbortTransactionAsync();

                return new FailureResult(operationName, "Inventory SKU not added");
            }

        }
    }

}
