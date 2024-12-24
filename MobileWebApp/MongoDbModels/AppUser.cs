using System.Security.Claims;

using MongoDB.Bson;

namespace MobileWebApp.MongoDbModels;

public class AppUser
{
    //[BsonId]
    public ObjectId Id { get; set; }

    //[BsonElement("username")]
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = "somehash";

    public string PasswordSalt { get; set; } = "somesalt";
    
    public DateTime PasswordLastUpdateDateTime { get; set; } = DateTime.UtcNow;

    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdateDateTime { get; set; } = DateTime.UtcNow;

    public string[] Applications { get; set; } = [];

    public IList<Claim> Claims { get; set; } = [];

}