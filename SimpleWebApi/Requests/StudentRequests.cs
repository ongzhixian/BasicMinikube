using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebApi.Requests;


public record RegisterNewStudentRequest
{
    public RegisterNewStudentRequest(string identityCode, ushort educationLevel, string name)
    {
        IdentityCode = identityCode;
        EducationLevel = educationLevel;
        Name = name;
    }

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


public record UpdateStudentRequest : RegisterNewStudentRequest
{
    public UpdateStudentRequest(string identityCode, ushort educationLevel, string name) : base(identityCode, educationLevel, name)
    {
    }

    [Description("Id use by system to uniquely identify course")]
    [Required]
    public required string CourseId { get; set; }
}