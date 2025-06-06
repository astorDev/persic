using Amazon.S3;
using Amazon.S3.Model;

namespace Persic;

public static class PresignedUrlsExtensions
{
    public static string GetPresignedUrl(this AmazonS3Client client, GetPreSignedUrlRequest request)
    {
        var rawPresignedUrl = client.GetPreSignedURL(request);

        return client.Config.ServiceURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            ? rawPresignedUrl.Replace("https://", "http://")
            : rawPresignedUrl;
    }
    public static string GetPresignedUrl(this AmazonS3Client client, string bucketName, string key, DateTime expires, HttpVerb verb = HttpVerb.GET)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = expires,
            Verb = verb
        };

        return client.GetPresignedUrl(request);
    }

    public static string GetPresignedUrl(this AmazonS3Client client, string bucketName, string key, int expiresAfterMinutes = 10, HttpVerb verb = HttpVerb.GET)
        => client.GetPresignedUrl(bucketName, key, DateTime.UtcNow.AddMinutes(expiresAfterMinutes), verb);

    public static string GetPresignedUrl(this S3BucketClient bucket, string key, DateTime expires, HttpVerb verb = HttpVerb.GET)
        => bucket.Client.GetPresignedUrl(bucket.Name, key, expires, verb);

    public static string GetPresignedUrl(this S3BucketClient bucket, string key, int expiresAfterMinutes = 10, HttpVerb verb = HttpVerb.GET)
        => bucket.Client.GetPresignedUrl(bucket.Name, key, expiresAfterMinutes, verb);
}