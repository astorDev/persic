# S3 with C# and .NET 9: The Most Effective Way to Manage Files

> A Practical Guide - **No AWS or other cloud account required.** Utilizing the C# 11 feature - UTF-8 string literals

![The Best Way to Store Your Files](thumb.png)

Introduced by Amazon in 2006, S3 has since become the de facto standard for cloud file storage. Practically every cloud provider today offers an S3-compatible storage service (even though many fields in the S3 protocol still contain "AMZ" in their names). 

In this article, we will experiment with managing files in S3 using C#. We'll go step by step from the ground up, improving Amazon's library for our convenience along the way. Without further ado, let's set things up!

> Or jump straight to the [TL;DR](#tldr) at the end of this article for the final code examples.

## Setting Up Local S3 Environment: MinIO & Docker

## Initial .NET Code: Connecting to S3 (MinIO instance)

## Getting Familiar with S3: Improved Connection and Buckets

## The Main Thing: File Upload and Download

## Final Chores: Improving File Access Code

## TL;DR

In this article, we've investigated integrating S3 in C#. We've covered connecting to S3, initiating a bucket, writing and reading files from it.

Along the way, we've found a few inefficiencies in the Amazon library, so we have made our own helper classes: `S3Client` and `S3BucketClient`. You can use those helpers by installing the `Persic.Files` NuGet package. With the package in place, you should be able to communicate with S3 with ease. Here's a full `Program.cs` demonstrating that:

> The example assumes a locally deployed MinIO instance. Refer to the first section of this article for details. 

```csharp
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
```

The package, as well as this article, is part of a project called [persic](https://github.com/astorDev/persic), containing various DB-related tooling. Check it out on [GitHub](https://github.com/astorDev/persic), and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉