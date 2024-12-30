using MongoDB.Bson;

namespace MobileWebApp.MongoDbModels;

public class InventoryItem
{
    public ObjectId Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string QuanityUnit { get; set; } = "PIECE";
    
    public string Status { get; set; } = null!;
}