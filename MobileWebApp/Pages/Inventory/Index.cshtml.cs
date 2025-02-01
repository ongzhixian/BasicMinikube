using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Services;

namespace MobileWebApp.Pages.Inventory;

public class InventoryIndexPageModel : PageModel
{
    private readonly ILogger<InventoryIndexPageModel> logger;
    private readonly InventoryService inventoryItemService;
    private readonly SessionService sessionService;
    private readonly AppSettingService appSettingService;

    // public IEnumerable<ItemSummary> InventoryItems { get; set; } = [];
    //public IEnumerable<InventoryItem> InventoryItems { get; set; } = [];
    public PageOf<InventoryItem> InventoryItems { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public InventoryIndexPageModel(ILogger<InventoryIndexPageModel> logger, InventoryService inventoryItemService,
        SessionService sessionService, AppSettingService appSettingService)
    {
        this.logger = logger;
        this.inventoryItemService = inventoryItemService;
        this.sessionService = sessionService;
        this.appSettingService = appSettingService;
    }

    public async Task OnGetAsync(int pageNumber = 1, int? pageSize = null)
    {
        //InventoryItems = await inventoryItemService.GetAllInventoryItemsAsync();
        
        var preferredPageSize = await sessionService.GetInt32Async(UserPreferences.PREFERRED_PAGE_SIZE);
        
        pageSize ??= preferredPageSize ??= appSettingService.DEFAULT_PAGE_SIZE;

        InventoryItems = await inventoryItemService.GetAllInventoryItemsAsync(pageNumber, pageSize.Value);

        PageNumber = pageNumber;    // pn
        PageSize = pageSize.Value;  // ps

        //InventoryItems = await inventoryItemService.GetInventoryItemQuantitiesAsync();

    }
    
}
