using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebApi.Requests;

// SubjectCode  Level   Name                    Title
// Psyc         220     Social Psychology       Psyc 220    Social Psychology
// ENG          1       English                 Engl   1    English

// https://www.moe.gov.sg/primary/curriculum/syllabus
public record NewCourseRequest
{
    public NewCourseRequest(string subjectCode, ushort level, string name)
    {
        SubjectCode = subjectCode;
        Level = level;
        Name = name;
    }

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


public record UpdateCourseRequest : NewCourseRequest
{
    public UpdateCourseRequest(string subjectCode, ushort level, string name) : base(subjectCode, level, name)
    {
    }

    [Description("Id use by system to uniquely identify course")]
    [Required]
    public required string CourseId { get; set; }
}