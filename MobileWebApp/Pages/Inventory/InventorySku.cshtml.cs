using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MobileWebApp.Models;
using MobileWebApp.MongoDbModels;
using MobileWebApp.Services;

namespace MobileWebApp.Pages;

public class InventorySkuPageModel : PageModel
{
    private readonly ILogger<InventorySkuPageModel> _logger;
    private readonly InventoryService inventoryService; 

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public string ItemName { get; set; } = string.Empty;

    [BindProperty(Name = "pn", SupportsGet = true)]
    public string PrevPageNumber { get; set; }

    [BindProperty(Name = "ps", SupportsGet = true)]
    public string PrevPageSize { get; set; }

    //public List<InventorySku> SkuList { get; set; }
    public PageOf<InventorySku> SkuList { get; set; }

    public InventorySkuPageModel(ILogger<InventorySkuPageModel> logger, InventoryService inventoryService)
    {
        _logger = logger;
        this.inventoryService = inventoryService;
    }

    public async Task OnGetAsync(string id, int pageNumber = 1, int pageSize = 5, int? pn = null, int? ps = null)
    {
        string itemName = id;
        var inventoryItem = await inventoryService.GetInventoryItemAsync(itemName);

        if (inventoryItem != null)
        {
            ItemName = inventoryItem.Name;
            SkuList = await inventoryService.GetSkusByItemNameAsync(inventoryItem.Name, pageNumber, pageSize);
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await inventoryService.AddNewItemAsync(ItemName);
                ViewData["message"] = "Item added";
            }
            catch (Exception ex)
            {

                ViewData["message"] = "Cannot add item " + ex.Message;
            }
            

            //EmailMessageModel testEmail = new EmailMessageModel
            //{
            //    Sender = new Email
            //    {
            //        Address = "MS_1puDAp@trial-z86org80oq04ew13.mlsender.net",
            //        Name = applicationTitle
            //    },
            //    Recipients = [new() { Address = Recipient }],
            //    Subject = "Test email message",
            //    Html = EmailBody
            //};

            //var emailServiceResponse = await emailService.SendEmailAsync(testEmail);

            //ViewData["message"] = $"{emailServiceResponse}";
        }

        return Page();
    }
}
