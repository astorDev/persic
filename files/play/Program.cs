using Amazon.S3;
using Amazon.S3.Model;
using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Services.AddS3FromConfiguration()
    .WithBucket("my-web-app");

var app = builder.Build();

var bucket = app.Services.GetRequiredService<S3BucketClient>();

//await bucket.EnsureInited();

app.MapPut("/upload", (S3Client client, GetPreSignedUrlRequest request) => new
{
    Url = client.GetPresignedUrl(request)
});

app.MapGet("/upload-mini", (S3Client client) => new
{
    Url = client.GetPresignedUrl("my-web-app", "upload-mini", TimeSpan.FromMinutes(1), HttpVerb.PUT)
});

app.MapGet("/download-mini", (S3Client client) => new
{
    Url = client.GetPresignedUrl("my-web-app", "upload-mini", TimeSpan.FromMinutes(1), HttpVerb.GET)
});

app.MapGet("pdf/download", (S3BucketClient bucket) => new
{
    Url = bucket.GetPresignedUrl("pdf", contentType: "application/pdf")
});

app.MapGet("pdf/upload", (S3BucketClient bucket) => new
{
    Url = bucket.GetPresignedUrl("pdf", verb: HttpVerb.PUT, contentType: "application/pdf")
});

app.Run();