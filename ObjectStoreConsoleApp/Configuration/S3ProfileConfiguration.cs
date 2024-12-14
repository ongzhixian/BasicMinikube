
using Amazon.Runtime;
using Amazon.S3;

namespace ObjectStoreConsoleApp.Configuration;

internal class ObjectStoreConfiguration
{
    public string SelectedProfile { get; set; } = string.Empty;

    public Dictionary<string, ObjectStoreProfile> Profiles { get; set; } = [];

    internal AmazonS3Config GetAmazonS3Config()
    {
        var s3ClientConfig = new AmazonS3Config
        {
            // ForcePathStyle
            // Forces requests to be sent to using path addressing style: http://localhost:19000/{bucket-name}
            // Otherwise by default it will use virtual host addressing style and send requests to:
            // http://{bucket-name}.localhost:19000
            // Which would failed with an error message of: 'No such host is known. (mybucket.localhost:19000)'
            // Reference: https://github.com/localstack/localstack/issues/7652
            ForcePathStyle = true,
            MaxErrorRetry = 1
            //RetryMode = RequestRetryMode.Standard,

            //ServiceURL = "http://localhost:19000",
            //ProxyHost = "localhost",
            //ProxyPort = 8083

            //Profile = new Amazon.Profile("minio")
            //RegionEndpoint = Amazon.RegionEndpoint.USEast1 // Set appropriate region if needed
        };

        var selectedProfile = Profiles[SelectedProfile];

        if (!string.IsNullOrEmpty(selectedProfile.ServiceUrl)) s3ClientConfig.ServiceURL = selectedProfile.ServiceUrl;
        if (!string.IsNullOrEmpty(selectedProfile.ProxyHost)) s3ClientConfig.ProxyHost = selectedProfile.ProxyHost;
        if (int.TryParse(selectedProfile.ProxyPort, out int proxyPort)) s3ClientConfig.ProxyPort = proxyPort;

        return s3ClientConfig;
    }

    internal BasicAWSCredentials GetBasicAWSCredentials(Microsoft.Extensions.Configuration.IConfigurationRoot configurationRoot)
    {
        string aws_access_key_id_config_key = $"{SelectedProfile}_aws_access_key_id";
        string aws_secret_access_key_config_key = $"{SelectedProfile}_aws_secret_access_key";

        string accessKeyID = configurationRoot[aws_access_key_id_config_key] ?? throw new NullReferenceException(aws_access_key_id_config_key);
        string secretAccessKeyID = configurationRoot[aws_secret_access_key_config_key] ?? throw new NullReferenceException(aws_secret_access_key_config_key);

        return new BasicAWSCredentials(accessKeyID, secretAccessKeyID);
    }
}

public class ObjectStoreProfile
{
    public string? ServiceUrl { get; set; }
    
    public string? ProxyHost { get; set; }
    
    public string? ProxyPort { get; set; }

    //"ServiceUrl": "http://localhost:19000",
    //        "ProxyHost": "localhost",
    //        "ProxyPort": "8083"
}
