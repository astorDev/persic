using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Persic.Files;

public class S3BucketClient(S3Client client, string bucketName)
{
    public Task<GetObjectResponse> GetObject(string key)
    {
        return client.GetObject(bucketName, key);
    }

    public Task<PutObjectResponse> PutObject(string key, Stream inputStream, string contentType)
    {
        return client.PutObject(bucketName, key, inputStream, contentType);
    }

    public string GetPresignedURL(string fileKey, DateTime expires)
    {
        return client.GetPresignedUrl(bucketName, fileKey, expires);
    }

    public string GetPresignedURL(string fileKey, int expiresAfterMinutes = 10)
    {
        return client.GetPresignedUrl(bucketName, fileKey, expiresAfterMinutes);
    }
}

public class S3Client(S3Configuration configuration)
{
    public S3Client(string rawConnectionString)
        : this(S3Configuration.Parse(rawConnectionString))
    {
    }

    public S3BucketClient Bucket(string bucketName)
    {
        return new S3BucketClient(this, bucketName);
    }

    private readonly AmazonS3Client innerClient = new(
        configuration.AccessKeyId,
        configuration.SecretAccessKey,
        new AmazonS3Config
        {
            ServiceURL = configuration.ServiceURL,
            ForcePathStyle = configuration.ForcePathStyle
        }
    );

    public Task<ListBucketsResponse> ListBuckets()
    {
        return innerClient.ListBucketsAsync();
    }

    public Task<GetObjectResponse> GetObject(string bucketName, string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };
        return innerClient.GetObjectAsync(request);
    }

    public string GetPresignedUrl(string bucketName, string key, DateTime expires)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = expires,
            Verb = HttpVerb.GET
        };

        return GetPresignedUrl(request);
    }

    public string GetPresignedUrl(string bucketName, string key, int expiresAfterMinutes = 10)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expiresAfterMinutes),
            Verb = HttpVerb.GET
        };

        return GetPresignedUrl(request);
    }

    private string GetPresignedUrl(GetPreSignedUrlRequest request)
    {
        var rawPresignedUrl = innerClient.GetPreSignedURL(request);

        if (configuration.ServiceURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            return rawPresignedUrl.Replace("https://", "http://");
        }

        return rawPresignedUrl;
    }

    public Task<PutObjectResponse> PutObject(string bucketName, string key, Stream inputStream, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = inputStream,
            ContentType = contentType
        };
        return innerClient.PutObjectAsync(request);
    }
}

public static class S3SyncExtensions
{
    public static ListBucketsResponse ListBucketsSync(this S3Client client)
    {
        return client.ListBuckets().Sync();
    }

    public static GetObjectResponse GetObjectSync(this S3Client client, string bucketName, string key)
    {
        return client.GetObject(bucketName, key).Sync();
    }

    public static PutObjectResponse PutObjectSync(this S3Client client, string bucketName, string key, Stream inputStream, string contentType)
    {
        return client.PutObject(bucketName, key, inputStream, contentType).Sync();
    }

    public static T Sync<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
}

public record S3Configuration(
    string AccessKeyId,
    string SecretAccessKey,
    string ServiceURL,
    bool ForcePathStyle
)
{
    public static S3Configuration Parse(string rawConnectionString)
    {
        var connectionString = ConnectionString.Parse(rawConnectionString);
        return Parse(connectionString);
    }

    public static S3Configuration Parse(ConnectionString connectionString)
    {
        return new S3Configuration(
            connectionString.GetRequiredStringValue("AccessKeyId"),
            connectionString.GetRequiredStringValue("SecretAccessKey"),
            connectionString.GetRequiredStringValue("ServiceURL"),
            connectionString.GetBoolValue("ForcePathStyle", true)
        );
    }
}

public class ConnectionString(Dictionary<string, string> source) : Dictionary<string, string>(source)
{
    public static ConnectionString Parse(string rawConnectionString, string separator = ";", string keyValueSeparator = "=")
    {
        var parts = rawConnectionString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var dictionary = new Dictionary<string, string>();

        foreach (var part in parts)
        {
            var keyValue = part.Split(keyValueSeparator);
            if (keyValue.Length == 2)
            {
                dictionary[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        return new ConnectionString(dictionary);
    }

    public string GetRequiredStringValue(string key)
    {
        if (TryGetValue(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Key '{key}' not found in connection string.");
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        if (TryGetValue(key, out var value) && bool.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }
}

public static class S3Registration
{
    public static IServiceCollection AddS3(this IServiceCollection services, string rawConnectionString)
    {
        var configuration = S3Configuration.Parse(rawConnectionString);
        return services.AddSingleton(new S3Client(configuration));
    }
}