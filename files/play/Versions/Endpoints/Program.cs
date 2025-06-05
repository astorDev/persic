using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Services.AddS3("ServiceURL=http://localhost:9000;AccessKeyId=minio;SecretAccessKey=minioP@ssw0rd;ForcePathStyle=true");

var app = builder.Build();

app.MapGet("/invoice", async (S3Client client) =>
{
    var response = await client.GetObject("one", "invoice.pdf");

    return Results.File(
        response.ResponseStream,
        response.Headers.ContentType,
        "invoice.pdf"
    );
});

app.MapGet("/presigned/{filename}", (string filename, S3Client client) =>
{
    var presignedUrl = client.GetPresignedUrl("one", filename);
    return Results.Redirect(presignedUrl);
});

app.MapPost("/upload", async (HttpRequest request, S3Client client) =>
{
    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    if (file == null)
        return Results.BadRequest("No file uploaded.");

    var bucketName = "one";
    var fileName = file.FileName;

    using var stream = file.OpenReadStream();
    await client.PutObject(bucketName, fileName, stream, file.ContentType);

    return Results.Ok(new { fileName });
});

app.Run();