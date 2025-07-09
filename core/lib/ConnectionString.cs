using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Persic.Core;

public record ConnectionString(Dictionary<string, string> Parameters)
{
    public static ConnectionString Parse(string connectionString)
    {
        var parameters = connectionString
            .Split(';')
            .Select(p => p.Split('='))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0], p => p[1]);

        return new ConnectionString(parameters);
    }
}

public static class MappingExtensions
{
    public static T To<T>(this ConnectionString connectionString) where T : class, new()
    {
        IEnumerable<KeyValuePair<string, string?>> parameters = connectionString.Parameters.Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value));

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(parameters)
            .Build();

        IServiceCollection services = new ServiceCollection();

        services
            .AddOptions<T>()
            .Bind(configuration)
            .ValidateDataAnnotations();

        return services.BuildServiceProvider().GetRequiredService<IOptions<T>>().Value;
    }
}