# S3 with C# and .NET 9: The Most Effective Way to Manage Files

> A Practical Guide - **No AWS or other cloud account required.** Utilizing the C# 11 feature - UTF-8 string literals

![The Best Way to Store Your Files](thumb.png)

Introduced by Amazon in 2006, S3 has since become the de facto standard for cloud file storage. Practically every cloud provider today offers an S3-compatible storage service (even though many fields in the S3 protocol still contain "AMZ" in their names). 

In this article, we will experiment with managing files in S3 using C#. We'll go step by step from the ground up, improving Amazon's library for our convenience along the way. Without further ado, let's set things up!

> Or jump straight to the [TL;DR](#tldr) at the end of this article for the final code examples.

## Setting Up Local S3 Environment: MinIO & Docker

`compose.yml`

```yaml
services:
  minio:
    image: minio/minio:latest
    container_name: minio
    ports:
      - "9000:9000"      # S3 API
      - "9001:9001"      # MinIO Console
    environment:
      MINIO_ROOT_USER: minio
      MINIO_ROOT_PASSWORD: minioP@ssw0rd
    command: server /data --console-address ":9001"
    volumes:
      - minio-data:/data

volumes:
  minio-data:
```

```sh
docker compose up -d
```

[http://localhost:9001](http://localhost:9001):

![](minio.png)

## Initial .NET Code: Connecting to S3 (MinIO instance)

```sh
dotnet add package AWSSDK.S3
```

```csharp
var config = new AmazonS3Config
{
    ServiceURL = "http://localhost:9000",
    ForcePathStyle = true
};

var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);
```

```csharp
var response = await client.ListBucketsAsync()

Console.WriteLine("Received buckets list");

foreach (var bucket in response.Buckets)
    Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
```

```text
System.NullReferenceException: Object reference not set to an instance of an object.
```

```csharp
public static class S3ClientBucketMethodsExtensions
{
    public static async Task<ListBucketsResponse> ListBuckets(this AmazonS3Client client)
    {
        var response = await client.ListBucketsAsync();
        response.Buckets ??= [];
        return response;
    }
}
```

```csharp
var response = await client.ListBuckets();

Console.WriteLine("Received buckets list");

foreach (var bucket in response.Buckets)
    Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
```

## Getting Familiar with S3: Creating a Bucket

```csharp
await client.PutBucketAsync(new PutBucketRequest
{
    BucketName = "tests",
    UseClientRegion = true
});

Console.WriteLine("Bucket 'tests' created or already exists.");
```

```text
Bucket 'tests' created or already exists.
```

However, if we run the same script the second time we will get this nasty exception instead:

```text
Amazon.S3.Model.BucketAlreadyOwnedByYouException: Your previous request to create the named bucket succeeded and you already own it. ---> Amazon.Runtime.Internal.HttpErrorResponseException: Exception of type 'Amazon.Runtime.Internal.HttpEr
      rorResponseException' was thrown.
```

```csharp
public static async Task<PutBucketResponse> PutBucket(this AmazonS3Client client, PutBucketRequest request)
{
    try
    {
        return await client.PutBucketAsync(request);
    }
    catch (AmazonS3Exception ex) when (ex.ErrorCode == "BucketAlreadyExists" || ex.ErrorCode == "BucketAlreadyOwnedByYou")
    {
        return new PutBucketResponse();
    }
}
```

```csharp
await client.PutBucket(new PutBucketRequest
{
    BucketName = "tests",
    UseClientRegion = true
});

Console.WriteLine("Bucket 'tests' created or already exists.");
```

```text
Bucket 'tests' created or already exists.
Received buckets list
Bucket: tests, Created: 6/6/2025 6:42:11 AM
```

## Making Life Easier: S3Client and ConnectionString

```csharp
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
```

```csharp
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
```

```csharp
public partial class S3Client(S3Configuration configuration) : 
    AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, new AmazonS3Config
    {
        ServiceURL = configuration.ServiceURL,
        ForcePathStyle = configuration.ForcePathStyle
    })
{
    public S3Client(string rawConnectionString)
        : this(S3Configuration.Parse(rawConnectionString))
    {
    }
}
```

```csharp
var client = new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");
```

## The Main Thing: File Upload and Download

```csharp
public static class StreamExtensions
{
    public static string ReadAsString(this Stream stream, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        var result = reader.ReadToEnd();
        return result;
    }
}
```

```csharp
var content = "Hello from raw upload!"u8;

using var stream = new MemoryStream(content.ToArray());
var uploadRequest = new PutObjectRequest
{
    BucketName = "tests",
    Key = "raw-uploaded.txt",
    InputStream = stream,
    ContentType = "text/plain"
};

var response = await client.PutObjectAsync(uploadRequest);

var getRequest = new GetObjectRequest
{
    BucketName = "tests",
    Key = "raw-uploaded.txt"
};

var returned = await client.GetObjectAsync(getRequest);

var contentRead = returned.ResponseStream.ReadAsString();

Console.WriteLine("Content read: " + contentRead);
```

```csharp
public static async Task<GetObjectResponse> GetObject(this AmazonS3Client client, string bucketName, string key)
{
    var request = new GetObjectRequest
    {
        BucketName = bucketName,
        Key = key
    };

    return await client.GetObjectAsync(request);
}

public static Task<PutObjectResponse> PutObject(this AmazonS3Client client, 
    string bucketName, 
    string key, 
    Stream inputStream, 
    string contentType)
{
    var request = new PutObjectRequest
    {
        BucketName = bucketName,
        Key = key,
        InputStream = inputStream,
        ContentType = contentType
    };
    return client.PutObjectAsync(request);
}
```

```csharp
var content = "Hello from semiraw upload!"u8;

using var stream = new MemoryStream(content.ToArray());
var response = await client.PutObject(
    bucketName: "tests",
    key: "semiraw-uploaded.txt",
    stream,
    contentType: "text/plain"
);

var returned = await client.GetObject("tests", "semiraw-uploaded.txt");

var contentRead = returned.ResponseStream.ReadAsString();

Console.WriteLine("Content read: " + contentRead);
```

```csharp
public static async Task<PutObjectResponse> PutObject(this AmazonS3Client client,
    string bucketName, string key, byte[] inputBytes, string contentType)
{
    using var stream = new MemoryStream(inputBytes);
    return await client.PutObject(bucketName, key, stream, contentType);
}

public static Task<PutObjectResponse> PutObject(this AmazonS3Client client,
    string bucketName, string key, ReadOnlySpan<byte> span, string contentType)
{
    return client.PutObject(bucketName, key, span.ToArray(), contentType);
}
```

```csharp
await client.PutObject("tests", "simple-uploaded.txt", "Hello from simple upload!"u8, "text/plain");
```

## Final Chores: S3BucketClient & Other Helpers

```csharp
public static async Task<PutBucketResponse> PutBucket(this S3Client client, string bucketName) =>
    await client.PutBucket(new PutBucketRequest
    {
        BucketName = bucketName,
        UseClientRegion = true
    });
```

```csharp
public partial record S3BucketClient(AmazonS3Client Client, string Name)
{
    public async Task<PutBucketResponse> EnsureInited() => await Client.PutBucket(Name);
}
```

```csharp
public static async Task<S3BucketClient> PutBucketClient(this AmazonS3Client client, string bucketName)
{
    await client.PutBucket(bucketName);
    return new S3BucketClient(client, bucketName);
}
```

```csharp
public static class S3BucketClientObjectExtensions
{
    public static async Task<GetObjectResponse> GetObject(this S3BucketClient bucket, string key)
        => await bucket.Client.GetObject(bucket.Name, key);

    public static Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, Stream inputStream, string contentType)
        => bucket.Client.PutObject(bucket.Name, key, inputStream, contentType);

    public static async Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, byte[] inputBytes, string contentType)
        => await bucket.Client.PutObject(bucket.Name, key, inputBytes, contentType);

    public static Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, ReadOnlySpan<byte> span, string contentType)
        => bucket.Client.PutObject(bucket.Name, key, span.ToArray(), contentType);
}
```

```csharp
var client = new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");

var bucket = await client.PutBucketClient("tests");

await bucket.PutObject("simple-uploaded.txt", "Hello from simple upload!"u8, "text/plain");

var returned = await bucket.GetObject("simple-uploaded.txt");
```

## TL;DR

In this article, we've investigated integrating S3 in C#. We've covered connecting to S3, initiating a bucket, writing and reading files from it.

Along the way, we've found a few inefficiencies in the Amazon library, so we have made our own helper classes: `S3Client` and `S3BucketClient`. You can use those helpers by installing the `Persic.Files` NuGet package. With the package in place, you should be able to communicate with S3 with ease. Here's a full `Program.cs` demonstrating that:

> The example assumes a locally deployed MinIO instance. Refer to the first section of this article for details. 

```csharp
using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Services.AddS3("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true")
    .WithBucket("my-web-app");

var app = builder.Build();

var bucket = app.Services.GetRequiredService<S3BucketClient>();

await bucket.EnsureInited();

await bucket.PutObject("hello.txt", "Hello, S3!"u8, "text/plain");

var received = await bucket.GetObject("hello.txt");
var readText = received.ResponseStream.ReadAsString();

app.Logger.LogInformation("Received: '{readText}'. Web Console: {consoleUrl}",
    readText, "http://localhost:9001");

app.Run();
```

The package, as well as this article, is part of a project called [persic](https://github.com/astorDev/persic), containing various DB-related tooling. Check it out on [GitHub](https://github.com/astorDev/persic), and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉