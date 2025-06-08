using Amazon.S3;
using Confi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public partial class S3Client(S3Configuration configuration) : 
    AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, new AmazonS3Config
    {
        ServiceURL = configuration.ServiceURL,
        ForcePathStyle = configuration.ForcePathStyle
    })
{
    public S3Configuration Configuration { get; } = configuration;

    public S3Client(string rawConnectionString)
        : this(S3Configuration.Parse(rawConnectionString))
    {
    }
}

public record S3Configuration(
    string AccessKeyId,
    string SecretAccessKey,
    string ServiceURL,
    bool ForcePathStyle,
    Uri? ExposedBaseUrl = null
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
            connectionString.GetStringValue("AccessKeyId"),
            connectionString.GetStringValue("SecretAccessKey"),
            connectionString.GetStringValue("ServiceURL"),
            connectionString.GetBoolValue("ForcePathStyle", true),
            connectionString.Search("ExposedBaseUrl", s => s.ToUri())
        );
    }
}

public record S3RegistrationBuilder(IServiceCollection Services);

public static class S3Registration
{
    public static S3RegistrationBuilder AddS3(this IServiceCollection services, Func<IServiceProvider, S3Configuration> configurationFactory)
    {
        services.AddSingleton(provider =>
        {
            var configuration = configurationFactory(provider);
            return new S3Client(configuration);
        });

        return new S3RegistrationBuilder(services);
    }

    public static S3RegistrationBuilder AddS3(this IServiceCollection services, string rawConnectionString)
        => services.AddS3(_ => S3Configuration.Parse(rawConnectionString));

    public static S3RegistrationBuilder AddS3(this IServiceCollection services, S3Configuration configuration)
        => services.AddS3(_ => configuration);

    public static S3RegistrationBuilder AddS3FromConfiguration(this IServiceCollection services, string connectionStringConfigurationPath = "ConnectionStrings:S3") =>
        AddS3(services, provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var rawConnectionString = configuration.GetRequiredValue(connectionStringConfigurationPath);
            return S3Configuration.Parse(rawConnectionString);
        });
}