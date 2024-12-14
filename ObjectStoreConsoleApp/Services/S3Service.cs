
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

    internal async Task<PutObjectResponse> PutObjectIntoBucketAsync(string bucketName, string objectKey, string? localFilePath = null)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey
        };

        if (localFilePath != null && File.Exists(localFilePath))
        {
            putObjectRequest.InputStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            putObjectRequest.ContentType = ResolveContentType(localFilePath);
        }

        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        return await s3Client.PutObjectAsync(putObjectRequest);
    }

    internal async Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName)
    {
        using var s3Client = new AmazonS3Client(basicAWSCredentials, amazonS3Config);

        return await s3Client.DeleteBucketAsync(bucketName);
    }

    private string ResolveContentType(string localFilePath)
    {
        // TODO: KIV for now: To be placed in WareLogix.Protocol
        return System.Net.Mime.MediaTypeNames.Image.Png;
    }


}
