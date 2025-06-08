using System.Text;
using Amazon.S3;

namespace Persic.Files.Playground;

[TestClass]
public class Presign
{
    [TestMethod]
    public async Task Get()
    {
        var bucket = await Connect.TestBucket();

        var presignedUrl = bucket.GetPresignedUrl("presigned-uploaded.txt");

        Console.WriteLine("Presigned URL: " + presignedUrl);
    }

    [TestMethod]
    public async Task GetReplacedBase()
    {
        var client = new S3Client("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true;ExposedBaseUrl=http://public.com:4321;");
        var bucket = await client.PutBucketClient("tests");

        var presignedUrl = bucket.GetPresignedUrl("replaced-base.txt");

        Console.WriteLine("Presigned URL with replaced base: " + presignedUrl);
    }

    [TestMethod] public void GetReplacedBaseSync() => GetReplacedBase().Sync();

    [TestMethod]
    public async Task OneWay()
    {
        var bucket = await Connect.TestBucket();

        var presignedUrl = bucket.GetPresignedUrl("presigned-uploaded.txt", verb: HttpVerb.PUT);

        await new HttpClient().PutStringContent(presignedUrl, "Hello from presigned URL upload!", "text/plain");

        var returned = await bucket.GetObject("presigned-uploaded.txt");

        var contentRead = returned.ResponseStream.ReadAsString();

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from presigned URL upload!");
    }

    [TestMethod] public void GetSync() => Get().Sync();

    [TestMethod]
    public async Task TwoWay()
    {
        var bucket = await Connect.TestBucket();

        var uploadPresignedUrl = bucket.GetPresignedUrl("two-way-presigned-uploaded.txt", verb: HttpVerb.PUT);

        await new HttpClient().PutStringContent(uploadPresignedUrl, "Hello from presigned URL upload!", "text/plain");

        var downloadPresignedUrl = bucket.GetPresignedUrl("two-way-presigned-uploaded.txt", verb: HttpVerb.GET);

        var contentRead = await new HttpClient().GetStringAsync(downloadPresignedUrl);

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from presigned URL upload!");
    }
}

public static class UploadExtensions
{
    public static async Task<string> PutStringContent(this HttpClient client, Uri url, string content, string contentType = "text/plain")
    {
        var response = client.PutAsync(url, new StringContent(content, Encoding.UTF8, contentType)).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetStringContent(this HttpClient client, Uri url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}