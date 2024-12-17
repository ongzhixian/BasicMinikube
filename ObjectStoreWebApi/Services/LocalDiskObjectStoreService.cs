using System.Globalization;
using System.Net;
using System.Xml.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

using ObjectStoreWebApi.DbContexts;
using ObjectStoreWebApi.DbModels;

using static System.Net.Mime.MediaTypeNames;

namespace ObjectStoreWebApi.Services;

public enum ObjectStoreServiceResult
{
    Success = HttpStatusCode.OK,
    Conflict = HttpStatusCode.Conflict,
    Error = HttpStatusCode.InternalServerError
}

public interface IObjectStoreService
{
    // BUCKET METHODS

    ListAllMyBucketsResult ListBuckets();
    Task<ObjectStoreServiceResult> PutBucketAsync(string bucketName);
    Task<ObjectStoreServiceResult> RemoveBucketAsync(string bucketName);

    // BUCKET OBJECT METHODS

    Task<ObjectStoreServiceResult> PutObjectInBucketAsync(string bucketName, string objectKey, Stream body);

    Task<ListBucketResult> ListObjectsInBucketAsync(string bucketName, int pageSize = 1000, int continuationToken = 0);

    Task<ObjectStoreServiceResult> RemoveObjectFromBucket(string bucketName, string objectKey);
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
    private readonly string storageLocationRootPath;

    public LocalDiskObjectStoreService(IConfiguration configuration, ObjectStorageContext objectStoreContext)
    {
        this.objectStoreContext = objectStoreContext;

        storageLocationRootPath = configuration["StorageService:StorageLocationRoot"] ?? AppContext.BaseDirectory;

        if (!Directory.Exists(storageLocationRootPath)) Directory.CreateDirectory(storageLocationRootPath);
    }

    public async Task<ObjectStoreServiceResult> PutObjectInBucketAsync(string bucketName, string objectKey, Stream body)
    {
        var bucket = await objectStoreContext.StorageBuckets.FirstOrDefaultAsync(r => r.Name == bucketName);

        if (bucket == null) return ObjectStoreServiceResult.Conflict;

        var storageObject = await objectStoreContext.StorageObjects.FirstOrDefaultAsync(r => r.BucketId == bucket.Id && r.Key == objectKey);

        string newStorageObjectPath;

        if (storageObject == null)
        {
            newStorageObjectPath = Path.Combine(storageLocationRootPath, bucketName, Guid.NewGuid().ToString());

            var newStorageObject = new StorageObject
            {
                BucketId = bucket.Id,
                Key = objectKey,
                StoragePath = newStorageObjectPath,
                Size = (int)body.Length,
                ModifiedDatetime = DateTime.UtcNow.ToString("O")
            };

            await objectStoreContext.StorageObjects.AddAsync(newStorageObject);
        }
        else
        {
            storageObject.Size = (int)body.Length;
            storageObject.ModifiedDatetime = DateTime.UtcNow.ToString("O");

            newStorageObjectPath = storageObject.StoragePath!;
        }

        using var fs = new FileStream(newStorageObjectPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        await body.CopyToAsync(fs);
        fs.Close();

        await objectStoreContext.SaveChangesAsync();

        return ObjectStoreServiceResult.Success;
    }


    public async Task<ListBucketResult> ListObjectsInBucketAsync(string bucketName, int pageSize = 1000, int continuationToken = 0)
    {
        /*
 * < ListBucketResult xmlns = "http://s3.amazonaws.com/doc/2006-03-01/" >
 *      < Name > lab - bucket1 </ Name >
 *      < Prefix ></ Prefix >
 *      < KeyCount > 3 </ KeyCount >
 *      < MaxKeys > 5 </ MaxKeys >
 *      < IsTruncated > false </ IsTruncated >
 *      < Contents >
 *          < Key > Test2 / SubFolder2 /</ Key >
 *          < LastModified > 2024 - 12 - 12T02: 05:25.633Z </ LastModified >
 *          < ETag > &#34;d41d8cd98f00b204e9800998ecf8427e&#34;</ETag>
 *          <Size>0</Size>
 *          <StorageClass>STANDARD</StorageClass>
 *      </Contents>
 *      <Contents><Key>Test2/SubFolder2/camera.png</Key><LastModified>2024-12-12T02:07:52.513Z</LastModified><ETag>&#34;d41d8cd98f00b204e9800998ecf8427e&#34;</ETag><Size>0</Size><StorageClass>STANDARD</StorageClass></Contents>
 *      <Contents><Key>bucket-object1</Key><LastModified>2024-12-15T04:23:58.184Z</LastModified><ETag>&#34;ad286b189dd21e7f5b5928cfaa035523&#34;</ETag><Size>85019</Size><StorageClass>STANDARD</StorageClass></Contents>
 *  </ListBucketResult>
 */
        ListBucketResult listBucketResult = new ListBucketResult();
        listBucketResult.Name = bucketName;
        listBucketResult.Prefix = string.Empty;
        listBucketResult.MaxKeys = pageSize;
        
        var bucket = await objectStoreContext.StorageBuckets.FirstOrDefaultAsync(r => r.Name == bucketName);
        if (bucket == null)
            return listBucketResult;

        var totalObjectCount = await objectStoreContext.StorageObjects.CountAsync();

        listBucketResult.Contents = await objectStoreContext.StorageObjects.Where(r => r.BucketId == bucket.Id).Select(r => new ListEntry
        {
            Key = r.Key,
            LastModified = DateTime.Parse(r.ModifiedDatetime),
            ETag = r.Etag,
            Size = (long)(r.Size ?? 0),
            StorageClass =  StorageClass.STANDARD
        }).OrderBy(r => r.Key)
        .Skip(continuationToken)
        .Take(pageSize)
        .ToArrayAsync();

        return listBucketResult;
    }
    public Task<ObjectStoreServiceResult> RemoveObjectFromBucket(string bucketName, string objectKey)
    {
        throw new NotImplementedException();
    }

    // BUCKET METHODS

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
        var bucketPath = Path.Combine(storageLocationRootPath, bucketName);
        if (!Directory.Exists(bucketPath)) Directory.CreateDirectory(bucketPath);
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
        var bucketPath = Path.Combine(storageLocationRootPath, bucketName);
        if (Directory.EnumerateFiles(bucketPath).Any())
            return ObjectStoreServiceResult.Conflict;
        if (Directory.Exists(bucketPath)) Directory.Delete(bucketPath);
        objectStoreContext.StorageBuckets.Remove(existingBucket);
        await objectStoreContext.SaveChangesAsync();
        return ObjectStoreServiceResult.Success;
    }

    
}
