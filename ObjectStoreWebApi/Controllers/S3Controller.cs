using System.Net;

using Microsoft.AspNetCore.Mvc;

using ObjectStoreWebApi.Services;

using WareLogix.Models.S3Models;

namespace ObjectStoreWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class S3Controller : ControllerBase
{
    private readonly ILogger<S3Controller> logger;
    private readonly IObjectStoreService objectStoreService;

    public S3Controller(ILogger<S3Controller> logger, IObjectStoreService objectStoreService)
    {
        this.logger = logger;
        this.objectStoreService = objectStoreService;
    }

    // BUCKET LEVEL

    //[Route("/")]
    [HttpGet("/", Name = "GetBucketList")]
    public IResult GetBucketList()
    {
        var result = objectStoreService.ListBuckets();

        return TypedResults.Content(
            S3XmlSerializer.ToXml(result),
            System.Net.Mime.MediaTypeNames.Application.Xml,
            System.Text.Encoding.UTF8,
            (int)HttpStatusCode.OK);
    }

    //[Route("/")]
    [HttpPut("/{bucketName}", Name = "PutBucket")]
    public async Task<IResult> PutBucketAsync(string bucketName)
    {
        if (await objectStoreService.PutBucketAsync(bucketName) == ObjectStoreServiceResult.Conflict)
            return TypedResults.Conflict();

        return TypedResults.Ok();
    }

    //[Route("/")]
    [HttpDelete("/{bucketName}", Name = "DeleteBucket")]
    public async Task<IResult> DeleteBucketAsync(string bucketName)
    {
        if (await objectStoreService.RemoveBucketAsync(bucketName) == ObjectStoreServiceResult.Conflict)
            return TypedResults.Conflict();

        return TypedResults.Ok();
    }

    // BUCKET-OBJECT LEVEL

    //[Route("/{bucketName}")]
    [HttpGet("/{bucketName}", Name = "GetBucketObjectList")]
    public IResult GetBucketObjectList(string bucketName, string objectKey)
    {
        return TypedResults.Ok("TODO: GetBucketObjectList");
    }

    //[Route("/{bucketName}/{objectKey}")]
    [HttpPut("/{bucketName}/{*objectKey}", Name = "PutBucketObject")]
    public IResult PutBucketObject(string bucketName, string objectKey)
    {
        return TypedResults.Ok("TODO: GetBucketObjectList");
    }

    //[Route("/{bucketName}/{objectKey}")]
    [HttpDelete("/{bucketName}/{*objectKey}", Name = "DeleteBucketObject")]
    public IResult DeleteBucketObject(string bucketName, string objectName)
    {
        return TypedResults.Ok("TODO: DeleteBucketObject");
    }

    //[Route("/{bucketName}/{objectKey}")]
    [HttpGet("/{bucketName}/{*objectKey}", Name = "GetBucketObject")]
    public IResult GetBucketObject(string bucketName, string objectKey)
    {
        return TypedResults.Ok("TODO: GetBucketObject");
    }
}
