using Amazon.S3;
using Amazon.S3.Model;

namespace Persic;

public static class S3ObjectExtensions
{
    public static async Task<GetObjectResponse> GetObject(this AmazonS3Client client, string bucketName, string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        return await client.GetObjectAsync(request);
    }

    public static Task<PutObjectResponse> PutObject(this AmazonS3Client client, string bucketName, string key, Stream inputStream, string contentType)
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

    public static async Task<PutObjectResponse> PutObject(this AmazonS3Client client, string bucketName, string key, byte[] inputBytes, string contentType)
    {
        using var stream = new MemoryStream(inputBytes);
        return await client.PutObject(bucketName, key, stream, contentType);
    }

    public static Task<PutObjectResponse> PutObject(this AmazonS3Client client, string bucketName, string key, ReadOnlySpan<byte> span, string contentType)
    {
        return client.PutObject(bucketName, key, span.ToArray(), contentType);
    }

    public static async Task<GetObjectResponse> GetObject(this S3BucketClient bucket, string key)
        => await bucket.Client.GetObject(bucket.Name, key);

    public static Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, Stream inputStream, string contentType)
        => bucket.Client.PutObject(bucket.Name, key, inputStream, contentType);

    public static async Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, byte[] inputBytes, string contentType)
        => await bucket.Client.PutObject(bucket.Name, key, inputBytes, contentType);

    public static Task<PutObjectResponse> PutObject(this S3BucketClient bucket, string key, ReadOnlySpan<byte> span, string contentType)
        => bucket.Client.PutObject(bucket.Name, key, span.ToArray(), contentType);
}