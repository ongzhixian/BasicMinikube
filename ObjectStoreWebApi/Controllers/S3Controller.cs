using System.Net;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
    public async Task<IResult> GetBucketObjectListAsync(string bucketName)
    {
        // continuation-token=YnVja2V0LW9iamVjdDEwW21pbmlvX2NhY2hlOnYyLHJldHVybjpd&list-type=2&max-keys=5 HTTP/1.1
        if (int.TryParse(Request.Query["continuation-token"], out int page))
            page = 0;

        if (!int.TryParse(Request.Query["max-keys"], out int pageSize))
            pageSize = 1000;

        var result = await objectStoreService.ListObjectsInBucketAsync(bucketName, pageSize, page);

        return TypedResults.Content(
            S3XmlSerializer.ToXml(result),
            System.Net.Mime.MediaTypeNames.Application.Xml,
            System.Text.Encoding.UTF8,
            (int)HttpStatusCode.OK);
    }

    //[Route("/{bucketName}/{objectKey}")]
    [HttpPut("/{bucketName}/{*objectKey}", Name = "PutBucketObject")]
    public async Task<IResult> PutBucketObjectAsync(string bucketName, string objectKey)
    {
        //if (await objectStoreService.PutObjectInBucket(bucketName, objectKey, Request.Body) == ObjectStoreServiceResult.Conflict)
        //    return TypedResults.Conflict();

        foreach (KeyValuePair<string, StringValues> item in Request.Headers)
            Console.WriteLine($"{item.Key} : {item.Value}");

        // AWS S3 does not use chunked transfer-encoding in the standard HTTP
        // So we look for 'X-Amz-Decoded-Content-Length' header instead
        if (Request.Headers.ContainsKey("X-Amz-Decoded-Content-Length")) // Chunked transfer-encoding
        {
            using var ms = new MemoryStream();
            await ReadChunkedEncodedStreamAsync(Request.Body, ms);
            await objectStoreService.PutObjectInBucketAsync(bucketName, objectKey, ms);
        }
        else
        {
            await objectStoreService.PutObjectInBucketAsync(bucketName, objectKey, Request.Body);
        }

        return TypedResults.Ok();

        //return TypedResults.Ok("TODO: GetBucketObjectList");
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


    // PRIVATE METHODS

    async Task ReadChunkedEncodedStreamAsync(Stream inputStream, MemoryStream ms)
    {
        const int BUFFER_SIZE = 1024;
        byte[] buffer = new byte[BUFFER_SIZE];
        string line;
        
        do
        {
            line = await ReadCrLfLineAsync(inputStream);
            
            var lineEntries = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
            if (lineEntries.Length > 0)
            {
                int chunkSize = Convert.ToInt32(lineEntries[0], 16);
                int bytesRead = 0;
                int totalBytesRead = 0;
                int bytesToRead = BUFFER_SIZE;

                while (totalBytesRead < chunkSize)
                {
                    if (chunkSize - totalBytesRead < BUFFER_SIZE)
                        bytesToRead = chunkSize - totalBytesRead;

                    bytesRead = await inputStream.ReadAsync(buffer, 0, bytesToRead);
                    ms.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }

                await inputStream.ReadExactlyAsync(buffer, 0, 1); // read chunk-terminating 'CR'
                await inputStream.ReadExactlyAsync(buffer, 0, 1); // read chunk-terminating 'LF'
            }

        } while (!string.IsNullOrEmpty(line));

        ms.Position = 0;
    }

    async Task<string> ReadCrLfLineAsync(Stream stream)
    {
        var line = new StringBuilder();

        Span<byte> stackSpan = stackalloc byte[1];
        Memory<byte> buffer = stackSpan.ToArray();

        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            if (buffer.Span[0] == '\r')
            {
                if (((bytesRead = await stream.ReadAsync(buffer)) > 0) && (buffer.Span[0] == '\n'))
                    break;
                else
                    line.Append((char)buffer.Span[0]);
            }
            else
            {
                line.Append((char)buffer.Span[0]);
            }
        }

        return line.ToString();
    }
}
