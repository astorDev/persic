using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.StaticAssets;

namespace Persic.Files.Playground;

[TestClass]
public class Load
{
    [TestMethod]
    public async Task RawSimple()
    {
        var client = await Connect.RawClient();
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

        contentRead.ShouldBe("Hello from raw upload!");
    }

    [TestMethod]
    public async Task SemiRawSimple()
    {
        var client = await Connect.RawClient();
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

        contentRead.ShouldBe("Hello from semiraw upload!");
    }

    [TestMethod]
    public async Task RawPresignedUrl()
    {
        var client = await Connect.RawClient();
        var content = "Hello from raw presigned URL upload!";

        var getPresignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = "tests",
            Key = "raw-presigned-uploaded.txt",
            Expires = DateTime.UtcNow.AddMinutes(10),
            Verb = HttpVerb.PUT
        };

        var presignedUrl = client.GetPreSignedURL(getPresignedUrlRequest).Replace("https://", "http://");

        using var httpClient = new HttpClient();
        var response = await httpClient.PutAsync(presignedUrl, new StringContent(content, Encoding.UTF8, "text/plain"));

        response.EnsureSuccessStatusCode();

        var getRequest = new GetObjectRequest
        {
            BucketName = "tests",
            Key = "raw-presigned-uploaded.txt"
        };

        var returned = await client.GetObjectAsync(getRequest);

        var contentRead = returned.ResponseStream.ReadAsString();

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from raw presigned URL upload!");
    }

    [TestMethod]
    public async Task Simple()
    {
        var client = Connect.TestClient();

        await client.PutObject("tests", "simple-uploaded.txt", "Hello from simple upload!"u8, "text/plain");

        var returned = await client.GetObject("tests", "simple-uploaded.txt");

        var contentRead = returned.ResponseStream.ReadAsString();

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from simple upload!");
    }

    [TestMethod]
    public async Task BucketedSimple()
    {
        var bucket = await Connect.TestBucket();

        await bucket.PutObject("simple-uploaded.txt", "Hello from simple upload!"u8, "text/plain");

        var returned = await bucket.GetObject("simple-uploaded.txt");

        var contentRead = returned.ResponseStream.ReadAsString();

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from simple upload!");
    }

    [TestMethod]
    public async Task PresignedUrl()
    {
        var bucket = await Connect.TestBucket();

        var presignedUrl = bucket.GetPresignedUrl("presigned-uploaded.txt", verb: HttpVerb.PUT);

        await new HttpClient().PutStringContent(presignedUrl, "Hello from presigned URL upload!", "text/plain");

        var returned = await bucket.GetObject("presigned-uploaded.txt");

        var contentRead = returned.ResponseStream.ReadAsString();

        Console.WriteLine("Content read: " + contentRead);

        contentRead.ShouldBe("Hello from presigned URL upload!");
    }
    
    [TestMethod]
    public async Task TwoWaysPresignedUrl()
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
    public static async Task<string> PutStringContent(this HttpClient client, string url, string content, string contentType = "text/plain")
    {
        var response = client.PutAsync(url, new StringContent(content, Encoding.UTF8, contentType)).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetStringContent(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}