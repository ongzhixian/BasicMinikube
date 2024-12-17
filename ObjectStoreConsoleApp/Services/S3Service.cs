using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace ObjectStoreConsoleApp.Services;

internal class S3Service
{
    private readonly BasicAWSCredentials basicAWSCredentials;
    private readonly AmazonS3Config amazonS3Config;

    public S3Service(BasicAWSCredentials basicAWSCredentials, AmazonS3Config amazonS3Config)
    {
        this.basicAWSCredentials = basicAWSCredentials;
        this.amazonS3Config = amazonS3Config;
    }

    // BUCKET METHODS

    internal async Task<List<S3Bucket>> ListBucketsAsync()
    {
        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        var response = await s3Client.ListBucketsAsync();

        return response.Buckets;
    }

    internal async Task<PutBucketResponse> CreateBucketAsync(string bucketName)
    {
        PutBucketRequest putBucketRequest = new()
        {
            BucketName = bucketName
        };

        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        return await s3Client.PutBucketAsync(putBucketRequest);
    }

    internal async Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName)
    {
        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        return await s3Client.DeleteBucketAsync(bucketName);
    }

    // BUCKET OBJECT METHODS

    internal async Task<PutObjectResponse> PutObjectToBucket(string bucketName, string objectKey, Stream? inputDataStream = null, string? contentType = null)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            //UseChunkEncoding = false
        };

        if (inputDataStream != null) putObjectRequest.InputStream = inputDataStream;
        if (contentType != null) putObjectRequest.ContentType = contentType;

        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        return await s3Client.PutObjectAsync(putObjectRequest);
    }

    internal async Task<PutObjectResponse> PutObjectIntoBucketAsync(string bucketName, string objectKey, string? localFilePath = null)
    {

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey
        };

        if (localFilePath != null && File.Exists(localFilePath))
        {
            return await PutObjectToBucket(bucketName, objectKey,
                new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                ResolveContentType(localFilePath));

            //putObjectRequest.InputStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //putObjectRequest.ContentType = ResolveContentType(localFilePath);

        }

        return await PutObjectToBucket(bucketName, objectKey);
    }

    internal async Task<List<S3Object>> ListObjectsInBucketAsync(string bucketName)
    {
        List<S3Object> results = new List<S3Object>();

        ListObjectsV2Request listObjectsRequest;
        ListObjectsV2Response listObjectResponse;

        listObjectsRequest = new ListObjectsV2Request
        {
            BucketName = bucketName,
            MaxKeys = 5,
        };

        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        do
        {
            listObjectResponse = await s3Client.ListObjectsV2Async(listObjectsRequest);

            results.AddRange(listObjectResponse.S3Objects);

            // If the response is truncated, set the request ContinuationToken
            // from the NextContinuationToken property of the response.
            listObjectsRequest.ContinuationToken = listObjectResponse.NextContinuationToken;
        }
        while (listObjectResponse.IsTruncated);

        return results;
    }

    private string ResolveContentType(string localFilePath)
    {
        // TODO: KIV for now: To be placed in WareLogix.Protocol
        return System.Net.Mime.MediaTypeNames.Image.Png;
    }

}
