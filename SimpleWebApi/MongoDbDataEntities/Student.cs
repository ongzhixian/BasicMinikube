using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SimpleWebApi.DataEntities;

public record Student
{
    [Description("Id use by system to uniquely identify student")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [Description("Code that represents student's nationality registration. For example NRIC number, passport number, foreign identification number.")]
    [Required]
    public required string IdentityCode { get; set; }

    [Description("Name of student")]
    [Required]
    public required string Name { get; set; }

    [Description("Education level of student. For example 10 grade 10")]
    [Required, Range(1, ushort.MaxValue)]
    public required ushort EducationLevel { get; set; }
    
}
