using System.Globalization;
using System.Net;

using Microsoft.EntityFrameworkCore;

using ObjectStoreWebApi.DbContexts;
using ObjectStoreWebApi.DbModels;

namespace ObjectStoreWebApi.Services;

public enum ObjectStoreServiceResult
{
    Success = HttpStatusCode.OK,
    Conflict = HttpStatusCode.Conflict,
    Error = HttpStatusCode.InternalServerError
}

public interface IObjectStoreService
{
    ListAllMyBucketsResult ListBuckets();
    //Task PutBucketAsync(string bucketName);
    Task<ObjectStoreServiceResult> PutBucketAsync(string bucketName);
    Task<ObjectStoreServiceResult> RemoveBucketAsync(string bucketName);
    //void AddBucket();
    //void RemoveBucket();

    //void AddObjectToBucket(string bucketName, string objectKey);
    //void RemoveObjectFromBucket(string bucketName, string objectKey);
    //void GetObjectFromBucket(string bucketName, string objectKey);
    //void ListObjectsInBucket(string bucketName, int pageSize);
}

public class LocalDiskObjectStoreService : IObjectStoreService
{
    private readonly ObjectStorageContext objectStoreContext;

    public LocalDiskObjectStoreService(ObjectStorageContext objectStoreContext)
    {
        this.objectStoreContext = objectStoreContext;
    }

    public ListAllMyBucketsResult ListBuckets()
    {
        ListAllMyBucketsResult result = new ListAllMyBucketsResult();

        result.Owner = new CanonicalUser();

        result.Buckets = objectStoreContext.StorageBuckets.Select(static r => new ListAllMyBucketsEntry
        {
            Name = r.Name,
            CreationDate = DateTime.Parse(r.CreateDatetime, CultureInfo.InvariantCulture)
        }).ToArray();

        return result;
    }

    public async Task<ObjectStoreServiceResult> PutBucketAsync(string bucketName)
    {
        if (objectStoreContext.StorageBuckets.Any(r => r.Name == bucketName))
            return ObjectStoreServiceResult.Conflict;

        var newBucket = new StorageBucket
        {
            Name = bucketName,
            CreateDatetime = DateTime.UtcNow.ToString("O")
        };

        objectStoreContext.StorageBuckets.Add(newBucket);
        await objectStoreContext.SaveChangesAsync();

        return ObjectStoreServiceResult.Success;
    }

    public async Task<ObjectStoreServiceResult> RemoveBucketAsync(string bucketName)
    {
        var existingBucket = await objectStoreContext.StorageBuckets.FirstOrDefaultAsync(r => r.Name == bucketName);

        if (existingBucket == null)
            return ObjectStoreServiceResult.Conflict;

        
        objectStoreContext.StorageBuckets.Remove(existingBucket);
        await objectStoreContext.SaveChangesAsync();

        return ObjectStoreServiceResult.Success;
    }
}
