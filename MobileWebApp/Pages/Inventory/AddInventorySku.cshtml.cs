using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.Repositories;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class AddInventorySkuPageModel : PageModel
{
    private readonly ILogger<AddInventorySkuPageModel> _logger;
    private readonly InventoryService inventoryService; 

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty, Required]
    [DataType(DataType.Text)]
    [Display(Name = "Increase Quantity")]
    public decimal ItemQuantity { get; set; }

    [BindProperty]
    public string ItemName { get; set; } = string.Empty;

    public AddInventorySkuPageModel(ILogger<AddInventorySkuPageModel> logger, InventoryService inventoryService)
    {
        _logger = logger;
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
                var inventoryItem = await inventoryService.GetInventoryItemAsync(ItemName);

                if (inventoryItem != null)
                {
                    // ItemName
                    await inventoryService.IncreaseItemQuantityAsync(inventoryItem.Name, ItemQuantity);
                    ViewData["message"] = "Item added";
                }

                
            }
            catch (Exception ex)
            {

                ViewData["message"] = "Cannot add item " + ex.Message;
            }
            

        }

        return Page();
    }
}
