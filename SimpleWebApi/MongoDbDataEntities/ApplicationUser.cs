using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SimpleWebApi.DataEntities;

public record ApplicationUser
{
    [Description("Id use by system to uniquely identify application user")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [Description("Username of application user. Use as part of login credentials")]
    [Required]
    public required string Username { get; set; }

    [Description("Hash of application user's login credentials")]
    [Required]
    public required string PasswordHash { get; set; }

    [Description("Salt of application user's login credentials")]
    [Required]
    public required string PasswordSalt { get; set; }

}
