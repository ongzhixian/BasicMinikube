
using System.Net;

using Microsoft.OpenApi.Models;

namespace SimpleWebApi.RouteGroups;

public static class JsonWebTokenRouteGroup
{

    public static RouteGroupBuilder WithJsonWebTokenApi(this RouteGroupBuilder group)
    {
        group.WithTags("JsonWebToken");

        // ADD / UPDATE / DELETE
        group.MapPost("/", NewJsonWebToken).WithOpenApi(NewJsonWebTokenOperationDescription);
        //group.MapPut("/", UpdateCourse).WithOpenApi(UpdateCourseOperationDescription);
        //group.MapDelete("/{id}", RemoveCourse).WithOpenApi(RemoveCourseOperationDescription);

        // RETRIEVE ONE / ALL
        //group.MapGet("/", GetHealth).WithOpenApi(GetHealthOperationDescription);

        return group;
    }


    // OPERATION: NewJsonWebToken

    private static OpenApiOperation NewJsonWebTokenOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "NewJsonWebToken";
        operation.Summary = "Get a new Json Web Token (JWT)";
        operation.Description = @"Use for getting a Json Web Token (JWT).<br/>JWTs are need to access APIs that are secured.";

        //operation.RequestBody.Description = "A JSON entity describing the course subject code, level and name.";

        //operation.Responses.Clear();
        //operation.Responses.Add($"{(int)HttpStatusCode.OK}", new OpenApiResponse
        //{
        //    Description = $"{HttpStatusCode.OK}<br/>Course is registered."
        //});
        //operation.Responses.Add($"{(int)HttpStatusCode.BadRequest}", new OpenApiResponse
        //{
        //    Description = $"{HttpStatusCode.BadRequest}<br/>Request data is invalid."
        //});
        //operation.Responses.Add($"{(int)HttpStatusCode.Conflict}", new OpenApiResponse
        //{
        //    Description = $"{HttpStatusCode.Conflict}<br/>Course with the same subjectCode and level is registered."
        //});

        return operation;
    }

    private static Microsoft.AspNetCore.Http.HttpResults.Ok NewJsonWebToken(HttpContext context)
    {
        return TypedResults.Ok();
    }

}
