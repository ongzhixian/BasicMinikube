using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleWebApi;

//public class Api
//{
//}

//// Student

//public record Student
//{
//    public required string Id { get; set; }

//    public required string Name { get; set; }

//    public required string EducationLevel { get; set; }
//}


//  CRUD
// R


//public class StudentApi
//{
//    static List<Student> studentList = [
//        new Student { Id = "0", EducationLevel = "6", Name = "James"},
//        new Student { Id = "1", EducationLevel = "6", Name = "Jack"},
//        new Student { Id = "2", EducationLevel = "6", Name = "Jane"}
//    ];

//    public static void AddStudentApi(WebApplication app)
//    {
//        app.MapGet("/students", () =>
//        {
//            return TypedResults.Ok("OK");
//        })
//        .Accepts<string>("plain/text")
//        .Produces<string>(StatusCodes.Status200OK)
//        .WithOpenApi(operation =>
//        {
//            operation.OperationId = "GetStudents";
//            operation.Summary = "Get students list";
//            operation.Description = "Get list of all students";
//            operation.Deprecated = true;
//            operation.Tags = new List<OpenApiTag> { new() { Name = "Student" } };

//            //var parameter = operation.Parameters[0];
//            //parameter.Description = "The ID associated with the created Todo";

//            return operation;
//        });


//        app.MapGet("/student/{id}", Results<Ok<Student>, NotFound>(string id) =>
//        {
//            return studentList.FirstOrDefault(r => r.Id.Equals(id, StringComparison.OrdinalIgnoreCase)) is Student student
//             ? TypedResults.Ok(student)
//             : TypedResults.NotFound();
//        })
//        .Accepts<string>("plain/text")
//        .Produces<string>(StatusCodes.Status200OK)
//        .WithOpenApi(operation =>
//        {
//            operation.OperationId = "GetStudent";
//            operation.Summary = "Get student matching id";
//            operation.Description = "Get record of student matching id";
//            //operation.Deprecated = true;
//            operation.Tags = new List<OpenApiTag> { new() { Name = "Student" } };

//            var parameter = operation.Parameters[0];
//            parameter.Description = "The ID associated with student";

//            return operation;
//        });
//    }


//}
public class OperationFilter : IOperationFilter
{
    //public DocumentFilter() { Thread.Sleep(20); }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) { }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        throw new NotImplementedException();
    }
}


public class AuthResponsesOperationFilter : IOperationFilter
{
    //public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //{
    //    var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
    //        .Union(context.MethodInfo.GetCustomAttributes(true))
    //        .OfType<AuthorizeAttribute>();

    //    if (authAttributes.Any())
    //    {
    //        var securityRequirement = new OpenApiSecurityRequirement()
    //        {
    //            {
    //                // Put here you own security scheme, this one is an example
    //                new OpenApiSecurityScheme
    //                {
    //                    Reference = new OpenApiReference
    //                    {
    //                        Type = ReferenceType.SecurityScheme,
    //                        Id = "Bearer"
    //                    },
    //                    Scheme = "oauth2",
    //                    Name = "Bearer",
    //                    In = ParameterLocation.Header,
    //                },
    //                new List<string>()
    //            }
    //        };
    //        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
    //        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    //    }
    //}
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>();

        if (authAttributes != null && authAttributes.Any())
        {
            var securityRequirement = new OpenApiSecurityRequirement()
            {
                {
                    // Put here you own security scheme, this one is an example
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}