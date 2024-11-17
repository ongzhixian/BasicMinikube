using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SimpleWebApi.DataEntities;

public record Course
{
    [Description("Id use by system to uniquely identify course")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [Description("A short code to describe the subject. For example ENG for English")]
    [Required, MinLength(1), MaxLength(4)]
    public required string SubjectCode { get; set; }

    [Description("An integer that describes the education standard of course. For example 10 for grade 10 course")]
    [Required, Range(1, ushort.MaxValue)]
    public required ushort Level { get; set; }

    
    [Description("Descriptive name for the course. For example English for an course in English")]
    [Required]
    public required string Name { get; set; }
}
