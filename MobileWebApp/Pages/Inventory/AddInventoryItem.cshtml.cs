using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class AddInventoryItemPageModel : PageModel
{
    private readonly ILogger<AddInventoryItemPageModel> logger;
    private readonly InventoryService inventoryService; 

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Unit of Measurement")]
    public string QuantityUnit { get; set; } = "PIECE";

    public AddInventoryItemPageModel(ILogger<AddInventoryItemPageModel> logger, InventoryService inventoryService)
    {
        this.logger = logger;
        this.inventoryService = inventoryService;
    }

    public async Task OnGetAsync(string id)
    {
        var inventoryItem = await inventoryService.GetInventoryItemAsync(id);

        if (inventoryItem != null)
        {
            ItemName = inventoryItem.Name;
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await inventoryService.AddNewItemAsync(ItemName);
                ViewData["message"] = $"'{ItemName}' added";

                ModelState.Clear();
                ItemName = string.Empty;
            }
            catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Category == MongoDB.Driver.ServerErrorCategory.DuplicateKey)
            {
                ViewData["message"] = $"'{ItemName}' is registered already; Item not added.";
            }
            catch (Exception ex)
            {
                ViewData["message"] = "Cannot add item " + ex.Message;
            }
        }

        return Page();
    }
}
