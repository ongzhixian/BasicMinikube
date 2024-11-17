using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebApi.Requests;


public record RegisterNewApplicationUserRequest
{
    public RegisterNewApplicationUserRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Description("Username of application user")]
    [Required]
    public required string Username { get; set; }

    [Description("Password of application user")]
    [Required]
    public required string Password { get; set; }

}


//public record UpdateStudentRequest : RegisterNewStudentRequest
//{
//    public UpdateStudentRequest(string identityCode, ushort educationLevel, string name) : base(identityCode, educationLevel, name)
//    {
//    }

//    [Description("Id use by system to uniquely identify course")]
//    [Required]
//    public required string CourseId { get; set; }
//}