using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public partial class S3Client(S3Configuration configuration) : 
    AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, new AmazonS3Config
    {
        ServiceURL = configuration.ServiceURL,
        ForcePathStyle = configuration.ForcePathStyle
    })
{
    public S3Client(string rawConnectionString)
        : this(S3Configuration.Parse(rawConnectionString))
    {
    }
}

public record S3Configuration(
    string AccessKeyId,
    string SecretAccessKey,
    string ServiceURL,
    bool ForcePathStyle
)
{
    public static S3Configuration Parse(string rawConnectionString)
    {
        var connectionString = ConnectionString.Parse(rawConnectionString);
        return Parse(connectionString);
    }

    public static S3Configuration Parse(ConnectionString connectionString)
    {
        return new S3Configuration(
            connectionString.GetRequiredStringValue("AccessKeyId"),
            connectionString.GetRequiredStringValue("SecretAccessKey"),
            connectionString.GetRequiredStringValue("ServiceURL"),
            connectionString.GetBoolValue("ForcePathStyle", true)
        );
    }
}

public record S3RegistrationBuilder(IServiceCollection Services);

public static class S3Registration
{
    public static S3RegistrationBuilder AddS3(this IServiceCollection services, string rawConnectionString)
    {
        var configuration = S3Configuration.Parse(rawConnectionString);
        services.AddSingleton(new S3Client(configuration));

        return new S3RegistrationBuilder(services);
    }
}