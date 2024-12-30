using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Services;

namespace MobileWebApp.Pages.Inventory;

public class InventoryIndexPageModel : PageModel
{
    private readonly ILogger<InventoryIndexPageModel> logger;
    private readonly InventoryService inventoryItemService;

    // public IEnumerable<ItemSummary> InventoryItems { get; set; } = [];
    //public IEnumerable<InventoryItem> InventoryItems { get; set; } = [];
    public PageOf<InventoryItem> InventoryItems { get; set; }

    public InventoryIndexPageModel(ILogger<InventoryIndexPageModel> logger, InventoryService inventoryItemService)
    {
        this.logger = logger;
        this.inventoryItemService = inventoryItemService;
    }

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 5)
    {
        //InventoryItems = await inventoryItemService.GetAllInventoryItemsAsync();

        InventoryItems = await inventoryItemService.GetAllInventoryItemsAsync(pageNumber, pageSize);
        

        //InventoryItems = await inventoryItemService.GetInventoryItemQuantitiesAsync();

    }
}
