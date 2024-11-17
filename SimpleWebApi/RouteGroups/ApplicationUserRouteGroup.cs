using System.Net;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using SimpleWebApi.Requests;
using SimpleWebApi.Services;

namespace SimpleWebApi.RouteGroups;

public static class ApplicationUserRouteGroup
{

    public static RouteGroupBuilder WithApplicationUserApi(this RouteGroupBuilder group)
    {
        group.WithTags("Course");

        // ADD / UPDATE / DELETE
        group.MapPost("/", RegisterNewApplicationUser).WithOpenApi(RegisterNewApplicationUserOperationDescription);
        //group.MapPut("/", UpdateCourse).WithOpenApi(UpdateCourseOperationDescription);
        //group.MapDelete("/{id}", RemoveCourse).WithOpenApi(RemoveCourseOperationDescription);

        // RETRIEVE ONE / ALL
        //group.MapGet("/", GetAllCoursesAsync).WithOpenApi(GetAllCoursesOperationDescription);

        return group;
    }


    // OPERATION: RegisterNewApplicationUser

    private static OpenApiOperation RegisterNewApplicationUserOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "RegisterNewApplicationUser";
        operation.Summary = "Register a application user into the system";
        operation.Description = @"Use for registering a new application user into the system.<br/>";

        //operation.RequestBody.Description = "A JSON entity describing the education level and name of student.";

        //operation.Responses.Clear();
        //operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse
        //{
        //    Description = $"{HttpStatusCode.OK}<br/>Student is registered."
        //});
        //operation.Responses.Add($"{(int)HttpStatusCode.BadRequest}", new OpenApiResponse
        //{
        //    Description = $"{HttpStatusCode.BadRequest}<br/>Request data is invalid."
        //});

        return operation;
    }

    private static async Task<Results<Ok, Conflict<RegisterNewStudentRequest>>> RegisterNewApplicationUser(
        HttpContext context, [FromBody] RegisterNewStudentRequest request, StudentService studentService)
    {
        try
        {
            await studentService.RegisterNewStudentAsync(request);
            return TypedResults.Ok();
        }
        catch (MongoDB.Driver.MongoWriteException exception)
        {
            if (exception.InnerException is MongoDB.Driver.MongoBulkWriteException mongoBulkWriteException
                && mongoBulkWriteException.WriteErrors.Any(r => r.Category == MongoDB.Driver.ServerErrorCategory.DuplicateKey))
                return TypedResults.Conflict<RegisterNewStudentRequest>(request);

            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
