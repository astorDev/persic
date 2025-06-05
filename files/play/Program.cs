using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Services.AddS3("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true")
    .WithBucket("my-web-app");

var app = builder.Build();

var bucket = app.Services.GetRequiredService<S3BucketClient>();

await bucket.EnsureInited();

await bucket.PutObject("hello.txt", "Hello, S3!"u8, "text/plain");

var received = await bucket.GetObject("hello.txt");
var readText = received.ResponseStream.ReadAsString();

app.Logger.LogInformation("Received: '{readText}'. Web Console: {consoleUrl}",
    readText, "http://localhost:9001");

app.Run();