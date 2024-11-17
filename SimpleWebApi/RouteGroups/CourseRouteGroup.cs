using System.Net;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using SimpleWebApi.DataEntities;
using SimpleWebApi.Requests;
using SimpleWebApi.Services;

namespace SimpleWebApi.RouteGroups;

public static class CourseRouteGroup
{
    public static RouteGroupBuilder WithCourseApi(this RouteGroupBuilder group)
    {

        //group.MapGet("/", GetAllCourses)
        //        .WithOpenApi(operation =>
        //        {
        //            operation.OperationId = "GetAllCourses";
        //            operation.Summary = "This is a summary for GetAllCourses";
        //            operation.Description = "This is a description for GetAllCourses";
        //            //operation.Deprecated = true;
        //            //operation.Tags = new List<OpenApiTag> { new() { Name = "Course" } };
        //            //var parameter = operation.Parameters[0];
        //            //parameter.Description = "The ID associated with the created Todo";

        //            return operation;
        //        });

        //group.MapGet("/{id}", GetCourse).WithOpenApi(GetCourseOperationDescription);

        group.WithTags("Course");

        // ADD / UPDATE / DELETE
        group.MapPost("/", RegisterCourse).WithOpenApi(RegisterCourseOperationDescription);
        group.MapPut("/", UpdateCourse).WithOpenApi(UpdateCourseOperationDescription);
        group.MapDelete("/{id}", RemoveCourse).WithOpenApi(RemoveCourseOperationDescription);

        // RETRIEVE ONE / ALL
        group.MapGet("/", GetAllCoursesAsync).WithOpenApi(GetAllCoursesOperationDescription);

        return group;
    }


    // OPERATION: RemoveCourse

    private static OpenApiOperation RemoveCourseOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "RemoveCourse";
        operation.Summary = "Remove an existing course";
        operation.Description = @"Use for removing an existing course in the system.";

        operation.Parameters[0].Description = "Id of course to remove";

        operation.Responses.Clear();
        operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.OK}<br/>Course is removed."
        });
        operation.Responses.Add($"{(int)HttpStatusCode.NotFound}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.NotFound}<br/>Course with id."
        });

        return operation;
    }

    private static async Task RemoveCourse(string courseId, CourseService courseService)
    {
        await courseService.RemoveCourseAsync(courseId);
    }


    // OPERATION: UpdateCourse

    private static OpenApiOperation UpdateCourseOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "UpdateCourse";
        operation.Summary = "Update an existing course";
        operation.Description = @"Use for registering a new course into the system.<br/><code>subjectCode</code> and <code>level</code> pair are treated as unique key.";

        operation.RequestBody.Description = "A JSON entity describing the course subject code, level and name.";

        operation.Responses.Clear();
        operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.OK}<br/>Course is registered."
        });
        operation.Responses.Add($"{(int)HttpStatusCode.BadRequest}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.BadRequest}<br/>Request data is invalid."
        });
        operation.Responses.Add($"{(int)HttpStatusCode.Conflict}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.Conflict}<br/>Course with the same subjectCode and level is registered."
        });

        //operation.Responses.Add("200", new OpenApiResponse
        //{
        //    Description = "some response desc",
        //    //Links = new Dictionary<string, OpenApiLink>
        //    //{
        //    //    { "asd", new OpenApiLink{  } },
        //    //    { "qwe", new OpenApiLink{  } }
        //    //}
        //});

        //operation.Tags = new List<OpenApiTag> { new() { Name = "Course" } };
        //operation.Deprecated = true;

        //var parameter = operation.Parameters[0];
        //parameter.Description = "The ID associated with the created Todo";

        return operation;
    }

    private static async Task UpdateCourse([FromBody] UpdateCourseRequest request, CourseService courseService)
    {
        await courseService.UpdateCourseAsync(request);
    }


    // OPERATION: RegisterCourse

    private static OpenApiOperation RegisterCourseOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "RegisterCourse";
        operation.Summary = "Register a new course";
        operation.Description = @"Use for registering a new course into the system.<br/><code>subjectCode</code> and <code>level</code> pair are treated as unique key.";

        operation.RequestBody.Description = "A JSON entity describing the course subject code, level and name.";

        operation.Responses.Clear();
        operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse 
        {
            Description = $"{HttpStatusCode.OK}<br/>Course is registered."
        });
        operation.Responses.Add($"{(int)HttpStatusCode.BadRequest}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.BadRequest}<br/>Request data is invalid."
        });
        operation.Responses.Add($"{(int)HttpStatusCode.Conflict}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.Conflict}<br/>Course with the same subjectCode and level is registered."
        });

        //operation.Responses.Add("200", new OpenApiResponse
        //{
        //    Description = "some response desc",
        //    //Links = new Dictionary<string, OpenApiLink>
        //    //{
        //    //    { "asd", new OpenApiLink{  } },
        //    //    { "qwe", new OpenApiLink{  } }
        //    //}
        //});

        //operation.Tags = new List<OpenApiTag> { new() { Name = "Course" } };
        //operation.Deprecated = true;

        //var parameter = operation.Parameters[0];
        //parameter.Description = "The ID associated with the created Todo";

        return operation;
    }

    private static async Task<Results<Ok, Conflict<NewCourseRequest>>> RegisterCourse([FromBody] NewCourseRequest request, CourseService courseService)
    {
        try
        {
            await courseService.RegisterCourseAsync(request);
            return TypedResults.Ok();
        }
        catch (MongoDB.Driver.MongoWriteException exception)
        {
            if (exception.InnerException is MongoDB.Driver.MongoBulkWriteException mongoBulkWriteException
                && mongoBulkWriteException.WriteErrors.Any(r => r.Category == MongoDB.Driver.ServerErrorCategory.DuplicateKey))
                    return TypedResults.Conflict<NewCourseRequest>(request);

            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }


    // OPERATION: GetAllCourses

    private static OpenApiOperation GetAllCoursesOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "GetAllCourses";
        operation.Summary = "List courses";
        operation.Description = @"Returns a list of all the courses that are registered in the system.<br/>Currently there is no pagination which might result a large payload.";

        operation.Responses.Clear();
        operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse
        {
            Description = $"{HttpStatusCode.OK}<br/>List of courses registered in system."
        });

        return operation;
    }

    private static async Task<Ok<List<Course>>> GetAllCoursesAsync(CourseService courseService)
    {
        return TypedResults.Ok(await courseService.GetAllCoursesAsync());
    }


    // OPERATION: GetCourse

    private static OpenApiOperation GetCourseOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "GetCourse";
        operation.Summary = "This is a summary for GetCourse";
        operation.Description = "This is a description for GetCourse";

        //operation.Tags = new List<OpenApiTag> { new() { Name = "Course" } };
        //operation.Deprecated = true;

        //var parameter = operation.Parameters[0];
        //parameter.Description = "The ID associated with the created Todo";

        return operation;
    }

    //private static Results<Ok<Course>, NotFound> GetCourse(int id)
    //{
    //    return courseList.FirstOrDefault(r => r.Id.Equals(id)) is Course course
    //    ? TypedResults.Ok(course)
    //    : TypedResults.NotFound();
    //}

    //private static List<Course> GetAllCourses(HttpContext context)
    //{
    //    return courseList;
    //}
}
