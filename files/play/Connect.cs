using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Persic.Files.Playground;

[TestClass]
public class Connect
{
    [TestMethod]
    public async Task RawMinio()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);
        var response = await client.ListBucketsAsync();

        Console.WriteLine("Received buckets list");

        foreach (var bucket in response.Buckets)
            Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
    }

    [TestMethod]
    public void SyncedRawMinio() => RawMinio().Sync();

    [TestMethod]
    public async Task SemiRawMinio()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);

        var response = await client.ListBuckets();

        Console.WriteLine("Received buckets list");

        foreach (var bucket in response.Buckets)
            Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
    }

    [TestMethod]
    public void SyncedSemiRawMinio() => SemiRawMinio().Sync();

    [TestMethod]
    public void Minio()
    {
        var client = new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");

        var buckets = client.ListBucketsAsync().Sync();

        foreach (var bucket in buckets.Buckets)
            Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
    }

    [TestMethod]
    //[ExpectedException(typeof(BucketAlreadyOwnedByYouException))]
    public async Task RawWithBucket()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);

        await client.PutBucketAsync(new PutBucketRequest
        {
            BucketName = "tests",
            UseClientRegion = true
        });

        Console.WriteLine("Bucket 'tests' created or already exists.");
    }

    [TestMethod]
    //[ExpectedException(typeof(BucketAlreadyOwnedByYouException))]
    public async Task SemiRawWithBucket()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);

        await client.PutBucket(new PutBucketRequest
        {
            BucketName = "tests",
            UseClientRegion = true
        });

        Console.WriteLine("Bucket 'tests' created or already exists.");
    }

    [TestMethod]
    public async Task Bucket()
    {
        var client = new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");

        await client.PutBucketClient("tests");

        Console.WriteLine("Bucket 'tests' created or already exists.");
    }

    public static S3Client TestClient()
    {
        return new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");
    }

    public static async Task<S3BucketClient> TestBucket()
    {
        return await TestClient().PutBucketClient("tests");
    }

    public static async Task<AmazonS3Client> RawClient()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        var client = new AmazonS3Client("minio", "minioP@ssw0rd", config);

        try
        {
            await client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = "tests",
                UseClientRegion = true
            });
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "BucketAlreadyExists" || ex.ErrorCode == "BucketAlreadyOwnedByYou")
        {
            // Ignore if bucket already exists
        }

        return client;
    }
}