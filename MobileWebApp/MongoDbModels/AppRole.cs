using MongoDB.Bson;

namespace MobileWebApp.MongoDbModels;

public class AppRole
{
    //[BsonId]
    public ObjectId Id { get; set; }

    public string RoleName { get; set; } = null!;
}