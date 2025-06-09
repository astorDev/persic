using Amazon.S3;
using Amazon.S3.Model;

namespace Persic;

public partial class S3Client
{
    public Uri GetPresignedUrl(GetPreSignedUrlRequest request)
    {
        if (Configuration.ExposedBaseUrl is not null)
        {
            var signatureGeneratorConfiguration = Configuration with
            {
                ServiceURL = Configuration.ExposedBaseUrl.ToString(),
                ExposedBaseUrl = null
            };

            var signatureGenerator = new S3Client(signatureGeneratorConfiguration);
            return signatureGenerator.GetPresignedUrl(request);
        }

        var rawPresignedUrl = GetPreSignedURL(request);
        var uri = new Uri(rawPresignedUrl);

        if (Config.ServiceURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            return uri.With(scheme: "http");

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

    public static Uri With(
        this Uri uri,
        string? scheme = null
    )
    {
        var builder = new UriBuilder(uri);

        if (scheme is not null) builder.Scheme = scheme;

        return builder.Uri;
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
    public static Uri GetPresignedUrl(this S3Client client,
        string bucketName,
        string key,
        DateTime expires,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = expires,
            Verb = verb,
            ContentType = contentType
        };

        return client.GetPresignedUrl(request);
    }

    public static Uri GetPresignedUrl(this S3Client client,
        string bucketName,
        string key,
        TimeSpan expiresIn,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
    {
        return client.GetPresignedUrl(
            bucketName: bucketName,
            key: key,
            expires: DateTime.UtcNow.Add(expiresIn),
            verb: verb,
            contentType: contentType
        );
    }

    public static Uri GetPresignedUrl(this S3Client client,
        string bucketName,
        string key,
        int expiresInMinutes = 10,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
        => client.GetPresignedUrl(
            bucketName: bucketName,
            key: key,
            expiresIn: TimeSpan.FromMinutes(expiresInMinutes),
            verb: verb,
            contentType: contentType
        );

    public static Uri GetPresignedUrl(this S3BucketClient bucket,
        string key,
        DateTime expires,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
        => bucket.Client.GetPresignedUrl(
            bucket.Name,
            key,
            expires,
            verb,
            contentType: contentType
        );

    public static Uri GetPresignedUrl(this S3BucketClient bucket,
        string key,
        TimeSpan expiresIn,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
        => bucket.Client.GetPresignedUrl(
            bucket.Name,
            key,
            expiresIn,
            verb,
            contentType: contentType
        );

    public static Uri GetPresignedUrl(this S3BucketClient bucket,
        string key,
        int expiresInMinutes = 10,
        HttpVerb verb = HttpVerb.GET,
        string? contentType = null
    )
        => bucket.GetPresignedUrl(
            key,
            TimeSpan.FromMinutes(expiresInMinutes),
            verb,
            contentType: contentType
        );
}