using MongoDB.Bson;

namespace MobileWebApp.MongoDbModels;

[Obsolete]
public class LoanItem
{
    public ObjectId Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Status { get; set; } = null!;
}