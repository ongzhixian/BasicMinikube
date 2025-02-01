using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using MobileWebApp.Api;
using MobileWebApp.Models;
using MobileWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileWebApp.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class SkuController : ControllerBase
{
    private readonly ILogger<SkuController> logger;
    //private readonly AppUserService appUserService;
    private readonly InventoryService inventoryService;

    public SkuController(ILogger<SkuController> logger, InventoryService inventoryService)
    {
        this.logger = logger;
        this.inventoryService = inventoryService;
    }

    // POST api/<UserController>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddNewSku value)
    {
        try
        {
            var operationResult = await inventoryService.AddInventorySkuAsync(value.ItemName, value.SkuId);

            if (operationResult.IsSuccess) return new JsonResult(operationResult);

            return new ConflictObjectResult(operationResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex); //throw;
        }
    }

}

public class AddNewSku
{
    public string ItemName { get; set; }
    public string SkuId { get; set; }
}