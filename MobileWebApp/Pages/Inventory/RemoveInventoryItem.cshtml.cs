using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class RemoveInventoryItemPageModel : PageModel
{
    private readonly ILogger<RemoveInventoryItemPageModel> logger;
    private readonly InventoryService inventoryService; 

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Display(Name = "Unit of Measurement")]
    public string QuantityUnit { get; set; } = "PIECE";

    public RemoveInventoryItemPageModel(ILogger<RemoveInventoryItemPageModel> logger, InventoryService inventoryService)
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
            QuantityUnit = inventoryItem.QuanityUnit;
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var operationResult = await inventoryService.RemoveItemAsync(ItemName);

                ViewData["message"] = operationResult.Message;

                //if (operationResult.IsSuccess)
                //    ViewData["message"] = $"'{ItemName}' removed";
                //else
                //    ViewData["message"] = $"'{ItemName}' cannot be removed.";
            }
            catch (Exception ex)
            {
                ViewData["message"] = $"Cannot remove item; {ex.Message}";
            }
        }

        return Page();
    }
}
