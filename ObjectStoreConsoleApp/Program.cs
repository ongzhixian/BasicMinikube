using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.Extensions.Configuration;

//IAmazonS3 client = new AmazonS3Client();
//string bucketName = string.Empty;
//string filePath = string.Empty;
//string keyName = string.Empty;

AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
AWSConfigs.LoggingConfig.LogResponsesSizeLimit = int.MaxValue; //1024 * 1024 * 36;
AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
//AWSConfigs.AddTraceListener

IConfigurationRoot configurationRoot = GetConfiguration();

string accessKeyID = configurationRoot["minio_aws_access_key_id"] ?? throw new NullReferenceException("minio_aws_access_key_id");
string secretAccessKeyID = configurationRoot["minio_aws_secret_access_key"] ?? throw new NullReferenceException("minio_aws_secret_access_key");

BasicAWSCredentials awsCredentials = new BasicAWSCredentials(accessKeyID, secretAccessKeyID);

var s3ClientConfig = new AmazonS3Config
{
    ServiceURL = "http://localhost:19000",

    // ForcePathStyle
    // Forces requests to be sent to using path addressing style: http://localhost:19000/{bucket-name}
    // Otherwise by default it will use virtual host addressing style and send requests to:
    // http://{bucket-name}.localhost:19000
    // Which would failed with an error message of: 'No such host is known. (mybucket.localhost:19000)'
    // Reference: https://github.com/localstack/localstack/issues/7652
    ForcePathStyle = true,

    //ProxyHost = "localhost",
    //ProxyPort = 8083

    //Profile = new Amazon.Profile("minio")
    //RegionEndpoint = Amazon.RegionEndpoint.USEast1 // Set appropriate region if needed
};

string bucketName = "mybucket";

using (var s3Client = new AmazonS3Client(awsCredentials, s3ClientConfig))
{
    // Before
    await ListBuckets(s3Client, "Before we start");
    
    //await CreateBucketAsync(s3Client, bucketName);
    //await ListBuckets(s3Client, "After PutBucketAsync");

    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\bag.jpg");

    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\camera.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\clipper-board.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\cloud.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\feather.png");

    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\flower.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\large-lens.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\mountain.png");
    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\yellow-flower.png");

    //await PutObjectIntoBucketAsync(s3Client, bucketName, "C:\\src\\temp\\test-pics\\TestEmfPptx.pptx");

    await displayBucketContents("Bucket contents after PutObjectAsync", s3Client, bucketName);

    // Create a GetObject request
    //Console.WriteLine(">>>" + $"Getting object");
    //var getObjectRequest = new GetObjectRequest
    //{
    //    BucketName = bucketName,
    //    Key = "7334e2d3-a4e6-4a8e-b95d-7e660be76a3c",
    //};

    //// Issue request and remember to dispose of the response
    //using GetObjectResponse getObjectResponse = await s3Client.GetObjectAsync(getObjectRequest);
    
    //await getObjectResponse.WriteResponseStreamToFileAsync($"C:\\src\\temp\\getobject-{Guid.NewGuid().ToString("N")}.pptx", true, CancellationToken.None);


    //// Remove item from bucket
    //var deleteObjectRequest = new DeleteObjectRequest
    //{
    //    BucketName = bucketName,
    //    Key = "bag.jpg",
    //};

    //Console.WriteLine(">>>" + $"Deleting object");
    //await s3Client.DeleteObjectAsync(deleteObjectRequest);
    //deleteObjectRequest = new DeleteObjectRequest
    //{
    //    BucketName = bucketName,
    //    Key = "TestEmfPptx.pptx",
    //};

    //Console.WriteLine($"Deleting object: {keyName}");
    //await s3Client.DeleteObjectAsync(deleteObjectRequest);
    //await displayBucketContents("Bucket contents after DeleteObjectAsync", s3Client, bucketName);


    // Delete bucket
    //await RemoveBucketAsync(s3Client, bucketName);
    //await ListBuckets(s3Client, "After DeleteBucketAsync");
}



async Task PutObjectIntoBucketAsync(AmazonS3Client s3Client, string bucketName, string filePath)
{
    Console.WriteLine($"    [Putting {filePath} into bucket {bucketName}]");

    var putObjectRequest = new PutObjectRequest
    {
        BucketName = bucketName,
        Key = Guid.NewGuid().ToString(),
        InputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read),
        ContentType = System.Net.Mime.MediaTypeNames.Image.Png
        //FilePath = "C:\\src\\temp\\bag.jpg",
        
    };

    var response = await s3Client.PutObjectAsync(putObjectRequest);

    Console.WriteLine($">>> PutObjectResponse >>> {response}");

    
}

async Task RemoveBucketAsync(AmazonS3Client s3Client, string bucketName)
{
    Console.WriteLine($"    [Removing bucket {bucketName}]");

    var response = await s3Client.DeleteBucketAsync(bucketName);

    Console.WriteLine($">>> DeleteBucketResponse >>> {response}");
}

async Task CreateBucketAsync(AmazonS3Client s3Client, string bucketName)
{
    Console.WriteLine($"    [Putting bucket {bucketName}]");

    PutBucketRequest putBucketRequest = new PutBucketRequest();
    putBucketRequest.BucketName = bucketName;
    
    var response = await s3Client.PutBucketAsync(putBucketRequest);
    Console.WriteLine($">>> PutBucketResponse >>> {response}");
}


// PRIVATE METHODS

static IConfigurationRoot GetConfiguration()
{

    //Console.WriteLine($"Environment.CurrentDirectory is {Environment.CurrentDirectory}");
    //Console.WriteLine($"Directory.GetCurrentDirectory() is {Directory.GetCurrentDirectory()}");
    //Findings: They are the same for console app;

    var builder = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddUserSecrets("47df7034-c1c1-4b87-8373-89f5e42fc9ec")
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //A plain old console app does not have "env" (that comes from host builder)
        //Another way to check is use: https://stackoverflow.com/questions/1611410/how-to-check-if-a-app-is-in-debug-or-release
        //But that not strikes me particularly flexible either; 
        //KIV, until we find a better solution for simple console jobs
        //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()
        ;

    return builder.Build();
}

static async Task ListBuckets(AmazonS3Client s3Client, string comment)
{
    Console.WriteLine($"    [{comment}]");
    var response = await s3Client.ListBucketsAsync();

    Console.WriteLine($">>> {response.Buckets.Count} bucket(s) found:");
    response.Buckets.ForEach(bucket => Console.WriteLine($">>> {bucket.BucketName}"));

    Console.WriteLine($">>> ListBucketsResponse >>> {response}");
}

static async Task ListDirectoryBuckets(string v, AmazonS3Client s3Client)
{
    Console.WriteLine(v);

    ListDirectoryBucketsRequest req = new ListDirectoryBucketsRequest();

    var actionResponse = await s3Client.ListDirectoryBucketsAsync(req);
    Console.WriteLine(">>>" + $"{actionResponse.Buckets.Count} bucket(s) found in directory:");
    actionResponse.Buckets.ForEach(bucket =>
    {
        Console.WriteLine(bucket.BucketName);
    });
}

static async Task displayBucketContents(string v, AmazonS3Client s3Client, string bucketName)
{
    Console.WriteLine(">>>" + $"{v}: {bucketName}");

    ListObjectsV2Request listObjectsRequest;
    ListObjectsV2Response listObjectResponse;

    listObjectsRequest = new ListObjectsV2Request
    {
        BucketName = bucketName,
        MaxKeys = 5,
    };
    do
    {
        listObjectResponse = await s3Client.ListObjectsV2Async(listObjectsRequest);

        listObjectResponse.S3Objects
            .ForEach(obj => Console.WriteLine(">>>" + $"{obj.Key,-35}{obj.LastModified.ToShortDateString(),10}{obj.Size,10}"));

        // If the response is truncated, set the request ContinuationToken
        // from the NextContinuationToken property of the response.
        listObjectsRequest.ContinuationToken = listObjectResponse.NextContinuationToken;
    }
    while (listObjectResponse.IsTruncated);
}