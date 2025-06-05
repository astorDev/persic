using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public partial class S3Client
{
    public async Task<PutBucketResponse> PutBucket(PutBucketRequest request)
    {
        try
        {
            return await PutBucketAsync(request);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "BucketAlreadyExists" || ex.ErrorCode == "BucketAlreadyOwnedByYou")
        {
            return new PutBucketResponse();
        }
    }
}

public partial record S3BucketClient(S3Client Client, string Name)
{
    public async Task<PutBucketResponse> EnsureInited() => await Client.PutBucket(Name);
}

public static class BucketExtensions
{
    public static async Task<PutBucketResponse> PutBucket(this S3Client client, string bucketName) =>
        await client.PutBucket(new PutBucketRequest
        {
            BucketName = bucketName,
            UseClientRegion = true
        });

    public static S3BucketClient Bucket(this S3Client client, string bucketName)
    {
        return new S3BucketClient(client, bucketName);
    }

    public static S3RegistrationBuilder WithBucket(this S3RegistrationBuilder builder, string bucketName)
    {
        builder.Services.AddSingleton(sp => {
            var client = sp.GetRequiredService<S3Client>();
            return new S3BucketClient(client, bucketName);
        });

        return builder;
    }

    public static async Task<S3BucketClient> PutBucketClient(this S3Client client, string bucketName)
    {
        await client.PutBucket(bucketName);
        return new S3BucketClient(client, bucketName);
    }
}