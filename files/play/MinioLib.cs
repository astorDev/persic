using Amazon.S3.Model;

namespace Persic.Files.Playground;

[TestClass]
public class MinioLib
{
    private const string ConnectionString = "ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true";

    [TestMethod]
    public void Connect()
    {
        var client = new S3Client(ConnectionString);

        var response = client.ListBucketsSync();

        foreach (var bucket in response.Buckets)
        {
            Console.WriteLine($"Bucket: {bucket.BucketName}, Created: {bucket.CreationDate}");
        }
    }

    [TestMethod]
    public void GetInvoice()
    {
        var client = new S3Client(ConnectionString).Bucket("one");

        var returned = client.GetObject("invoice.pdf").Sync();

        PrintObjectResponse(returned);
    }

    [TestMethod]
    public void GetInvoicePresignedUrl()
    {
        var client = new S3Client(ConnectionString).Bucket("one");

        var presignedUrl = client.GetPresignedURL("invoice.pdf");

        Console.WriteLine($"Presigned URL: {presignedUrl}");
    }

    [TestMethod]
    public void SaveFile()
    {
        var client = new S3Client(ConnectionString);
        var content = "Hello, Minio!";

        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
        {
            client.PutObjectSync(
                "one",
                "test-file.txt",
                stream,
                "text/plain"
            );
        }

        var returned = client.GetObjectSync("one", "test-file.txt");

        using var responseStream = returned.ResponseStream;
        using var reader = new StreamReader(responseStream);
        var contentRead = reader.ReadToEnd();

        Console.WriteLine($"File content: {contentRead}, Headers: {returned.Headers}");

        foreach (var key in returned.Headers.Keys)
        {
            Console.WriteLine($"{key}: {returned.Headers[key]}");
        }
    }
    
    public static GetObjectResponse PrintObjectResponse(GetObjectResponse response)
    {
        using var responseStream = response.ResponseStream;
        using var reader = new StreamReader(responseStream);
        var contentRead = reader.ReadToEnd();

        Console.WriteLine($"File content: {contentRead}, Headers: {response.Headers}");

        foreach (var key in response.Headers.Keys)
        {
            Console.WriteLine($"{key}: {response.Headers[key]}");
        }

        return response;
    }
}