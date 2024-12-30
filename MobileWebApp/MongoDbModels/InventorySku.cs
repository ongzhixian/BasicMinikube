using MongoDB.Bson;

namespace MobileWebApp.MongoDbModels;

public class InventorySku 
{
    public ObjectId Id { get; set; }

    public string SkuId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string QuanityUnit { get; set; } = "PIECE";
    
    public string Status { get; set; } = null!;
}