using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace SimpleWebApi.RouteGroups;

public static class HealthRouteGroup
{

    public static RouteGroupBuilder WithHealthApi(this RouteGroupBuilder group)
    {
        group.WithTags("Health");

        // ADD / UPDATE / DELETE
        //group.MapPost("/", RegisterNewStudent).WithOpenApi(RegisterNewStudentOperationDescription);
        //group.MapPut("/", UpdateCourse).WithOpenApi(UpdateCourseOperationDescription);
        //group.MapDelete("/{id}", RemoveCourse).WithOpenApi(RemoveCourseOperationDescription);

        // RETRIEVE ONE / ALL
        group.MapGet("/", GetHealth).WithOpenApi(GetHealthOperationDescription);

        return group;
    }


    // OPERATION: GetHealth

    
    private static OpenApiOperation GetHealthOperationDescription(OpenApiOperation operation)
    {
        operation.OperationId = "GetHealth";
        operation.Summary = "Get health state of application";
        operation.Description = @"Use for checking the health state of application.<br/>";


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

    [AllowAnonymous]
    [Authorize]
    private static Microsoft.AspNetCore.Http.HttpResults.Ok GetHealth(HttpContext context)
    {
        return TypedResults.Ok();
    }
}
