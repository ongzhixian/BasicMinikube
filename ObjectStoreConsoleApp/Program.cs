using System.Text;

using Amazon.Runtime;
using Amazon.S3;

using Microsoft.Extensions.Configuration;

using ObjectStoreConsoleApp.Configuration;
using ObjectStoreConsoleApp.Services;

ConfigureAwsLogging();

IConfigurationRoot configurationRoot = GetConfiguration();
Console.WriteLine($"{configurationRoot}");

var objectStoreConfiguration = new ObjectStoreConfiguration();
configurationRoot.GetSection("ObjectStore").Bind(instance: objectStoreConfiguration);

Console.WriteLine($"AppContext.BaseDirectory {AppContext.BaseDirectory}");
Console.WriteLine($"AppDomain.CurrentDomain.BaseDirectory {AppDomain.CurrentDomain.BaseDirectory}");
Console.WriteLine($"Environment.CurrentDirectory is {Environment.CurrentDirectory}");
Console.WriteLine($"Directory.GetCurrentDirectory() is {Directory.GetCurrentDirectory()}");
Console.WriteLine($"Selected profile: {objectStoreConfiguration.SelectedProfile}");

BasicAWSCredentials awsCredentials = objectStoreConfiguration.GetBasicAWSCredentials(configurationRoot);

AmazonS3Config s3ClientConfig = objectStoreConfiguration.GetAmazonS3Config();

string bucketName = "mybucket";
bucketName = "lab-bucket1";

var s3 = new S3Service(awsCredentials, s3ClientConfig);

// SCRIPT

// await RunBucketTestScriptAsync();

await RunBucketObjectTestScriptAsync();

Console.WriteLine("[All done]");

return;

async Task RunBucketObjectTestScriptAsync()
{
    try
    {
        await ListBuckets(s3, "[RunBucketObjectTestScriptAsync]");

        //await AddBucket(s3, bucketName);

        //await ListBuckets(s3, "after add bucket");

        //int currentLine = 0;
        //StringBuilder sb = new StringBuilder();
        //while (sb.Length < (1024 * 83))
        //{
        //    sb.AppendLine($"Hello world from bucket-object; Line {currentLine++}");
        //}
        ////Console.WriteLine($"Message has {currentLine} lines");

        //for (int i = 0; i < 15; i++)
        //{
        //    await PutObjectToBucketAsync(s3, bucketName, $"bucket-object{i}",
        //        new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));
        //}

        //await PutObjectToBucketAsync(s3, bucketName, "bucket-object1",
        //    new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));

        await ListObjectsInBucketAsync(s3, bucketName, "bucket-object1");

    }
    catch (Exception)
    {

        throw;
    }
    finally
    {
        //await RemoveBucket(s3, bucketName);

        //await ListBuckets(s3, "after remove bucket");
    }
}


// SCRIPTS

async Task PutObjectToBucketAsync(S3Service s3, string bucketName, string objectKey, Stream inputDataStream, string? comment = null)
{
    Console.Write($"[{nameof(PutObjectToBucketAsync)}]");
    if (comment == null)
        Console.WriteLine();
    else
        Console.WriteLine($" - {comment}");

    var response = await s3.PutObjectToBucket(bucketName, objectKey, inputDataStream);

    Console.WriteLine($"[{nameof(PutObjectToBucketAsync)}] received {response}");
    Console.WriteLine();
}

async Task ListObjectsInBucketAsync(S3Service s3, string bucketName, string objectKey, string? comment = null)
{
    Console.Write($"[{nameof(ListObjectsInBucketAsync)}]");
    if (comment == null)
        Console.WriteLine();
    else
        Console.WriteLine($" - {comment}");

    var response = await s3.ListObjectsInBucketAsync(bucketName);

    Console.WriteLine($"[{nameof(ListObjectsInBucketAsync)}] received {response} ; {response.Count} object(s)");
    
    //foreach (var item in response)
    //{
    //    Console.WriteLine(item);
    //}

    Console.WriteLine();
}

async Task RunBucketTestScriptAsync()
{
    await ListBuckets(s3, "[RunBucketTestScriptAsync]");

    await AddBucket(s3, bucketName);

    await ListBuckets(s3, "after add bucket");

    await RemoveBucket(s3, bucketName);

    await ListBuckets(s3, "after remove bucket");

}


static async Task ListBuckets(S3Service s3, string? comment = null)
{
    Console.Write($"[{nameof(ListBuckets)}]");
    if (comment == null)
        Console.WriteLine();
    else
        Console.WriteLine($" - {comment}");

    var buckets = await s3.ListBucketsAsync();

    foreach (var item in buckets)
        Console.WriteLine(item.BucketName);

    Console.WriteLine();
}

static async Task AddBucket(S3Service s3, string bucketName, string? comment = null)
{
    Console.Write($"[{nameof(AddBucket)}]");
    if (comment == null)
        Console.WriteLine();
    else
        Console.WriteLine($" - {comment}");

    var response = await s3.CreateBucketAsync(bucketName);

    Console.WriteLine($"[{nameof(AddBucket)}] received {response}");
    Console.WriteLine();
}

static async Task RemoveBucket(S3Service s3, string bucketName, string? comment = null)
{
    Console.Write($"[{nameof(RemoveBucket)}]");
    if (comment == null)
        Console.WriteLine();
    else
        Console.WriteLine($" - {comment}");

    var response = await s3.DeleteBucketAsync(bucketName);

    Console.WriteLine($"[{nameof(RemoveBucket)}] received {response}");
    Console.WriteLine();
}


// PRIVATE METHODS
static void ConfigureAwsLogging()
{
    //AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
    //AWSConfigs.LoggingConfig.LogResponsesSizeLimit = int.MaxValue; //1024 * 1024 * 36;
    //AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;

    //AWSConfigs.AddTraceListener
}

static IConfigurationRoot GetConfiguration()
{
    //Console.WriteLine($"Environment.CurrentDirectory is {Environment.CurrentDirectory}");
    //Console.WriteLine($"Directory.GetCurrentDirectory() is {Directory.GetCurrentDirectory()}");
    //Findings: They are the same for console app; but we should really be using AppContext.BaseDirectory
    //          This is because if we run the executable in a different directory, CurrentDirectory will reference that location

    var builder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
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
