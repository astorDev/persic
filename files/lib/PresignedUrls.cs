using Amazon.S3;
using Amazon.S3.Model;

namespace Persic;

public partial class S3Client
{
    public Uri GetPresignedUrl(GetPreSignedUrlRequest request)
    {
        var rawPresignedUrl = GetPreSignedURL(request);
        var uri = new Uri(rawPresignedUrl);

        if (Configuration.ExposedBaseUrl is not null)
            return uri.WithBaseUrl(Configuration.ExposedBaseUrl);

        if (Config.ServiceURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            return new Uri(rawPresignedUrl.Replace("https://", "http://"));

        return uri;
    }
}

public static class UriExtensions
{
    public static Uri ToUri(this string uriString)
    {
        if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
        {
            return uri;
        }

        throw new ArgumentException($"Invalid URI: {uriString}");
    }

    public static Uri WithBaseUrl(this Uri uri, string baseUrl)
    {
        return uri.WithBaseUrl(new Uri(baseUrl));
    }

    public static Uri WithBaseUrl(this Uri uri, Uri baseUrl)
    {
        var builder = new UriBuilder(uri)
        {
            Port = baseUrl.Port,
            Scheme = baseUrl.Scheme,
            Host = baseUrl.Host
        };

        return builder.Uri;
    }
}

public static class PresignedUrlsExtensions
{
    public static Uri GetPresignedUrl(this S3Client client, string bucketName, string key, DateTime expires, HttpVerb verb = HttpVerb.GET)
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

    public static Uri GetPresignedUrl(this S3Client client, string bucketName, string key, int expiresAfterMinutes = 10, HttpVerb verb = HttpVerb.GET)
        => client.GetPresignedUrl(bucketName, key, DateTime.UtcNow.AddMinutes(expiresAfterMinutes), verb);

    public static Uri GetPresignedUrl(this S3BucketClient bucket, string key, DateTime expires, HttpVerb verb = HttpVerb.GET)
        => bucket.Client.GetPresignedUrl(bucket.Name, key, expires, verb);

    public static Uri GetPresignedUrl(this S3BucketClient bucket, string key, int expiresAfterMinutes = 10, HttpVerb verb = HttpVerb.GET)
        => bucket.Client.GetPresignedUrl(bucket.Name, key, expiresAfterMinutes, verb);
}