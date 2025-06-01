using Amazon.S3;
using Amazon.S3.Model;

namespace Persic.Files.Playground;

[TestClass]
public class Minio
{
    [TestMethod]
    public void Connect()
    {
        var client = MinioClientFactory.Create();

        var response = client.ListBucketsAsync().GetAwaiter().GetResult();

        foreach (var bucket in response.Buckets)
        {
            Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
        }
    }

    [TestMethod]
    public void GetInvoice()
    {
        var client = MinioClientFactory.Create();
        var bucketName = "one";
        var fileName = "invoice.pdf";

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileName
        };

        var returned = client.GetObjectAsync(request).GetAwaiter().GetResult();

        using var responseStream = returned.ResponseStream;
        using var reader = new StreamReader(responseStream);
        var contentRead = reader.ReadToEnd();

        Console.WriteLine($"File content: {contentRead}, Headers: {returned.Headers}");

        foreach (var key in returned.Headers.Keys)
        {
            Console.WriteLine($"{key}: {returned.Headers[key]}");
        }
    }

    [TestMethod]
    public void GetInvoicePresignedUrl()
    {
        var client = MinioClientFactory.Create();
        var bucketName = "one";
        var fileName = "invoice.pdf";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = fileName,
            Expires = DateTime.UtcNow.AddMinutes(10), // URL valid for 10 minutes
            Verb = HttpVerb.GET,
        };

        var presignedUrl = client.GetPreSignedURL(request).Replace("https", "http");

        Console.WriteLine($"Presigned URL: {presignedUrl}");

        // Optionally, you can test the URL by downloading the file using HttpClient
        using var httpClient = new HttpClient();
        var response = httpClient.GetAsync(presignedUrl).Sync();
        var content = response.Content.ReadAsStringAsync().Sync();

        Console.WriteLine($"Downloaded content: {content}");
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void SaveFile()
    {
        var client = MinioClientFactory.Create();
        var bucketName = "one";
        var fileName = "test-file.txt";
        var content = "Hello, Minio!";

        // Upload the file
        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = "text/plain"
            };

            client.PutObjectAsync(putObjectRequest).GetAwaiter().GetResult();
        }

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileName
        };

        var returned = client.GetObjectAsync(request).GetAwaiter().GetResult();

        using var responseStream = returned.ResponseStream;
        using var reader = new StreamReader(responseStream);
        var contentRead = reader.ReadToEnd();
        Console.WriteLine($"File content: {contentRead}, Headers: {returned.Headers}");
        foreach (var key in returned.Headers.Keys)
        {
            Console.WriteLine($"{key}: {returned.Headers[key]}");
        }
        Assert.AreEqual(content, contentRead);
    }
}

public class MinioClientFactory
{
    public static AmazonS3Client Create()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        return new AmazonS3Client("minio", "minioP@ssw0rd", config);
    }
}