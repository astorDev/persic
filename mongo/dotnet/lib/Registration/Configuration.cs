using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using Nist;

namespace Persic;

public class MongoConfigurationManager(IEnumerable<IMongoCollectionConfiguration> configurations)
{
    public async Task Apply()
    {
        foreach (var configuration in configurations)
        {
            await configuration.Apply();
        }
    }
}

public interface IMongoCollectionConfiguration
{
    Task Apply();
}

public partial record MongoRegistrationBuilder
{
    public MongoRegistrationBuilder AddConfiguration<T>() where T : class, IMongoCollectionConfiguration
    {
        Services.AddScoped<IMongoCollectionConfiguration, T>();

        return this;
    }

    public MongoRegistrationBuilder AddCollection<TCollection, TConfiguration>(string name) where TCollection : class where TConfiguration : class, IMongoCollectionConfiguration
    {
        Services.AddMongoCollection<TCollection>(name);
        Services.AddScoped<IMongoCollectionConfiguration, TConfiguration>();

        return this;
    }
}

public static class MongoConfigurationExtensions
{
    public static async Task ApplyMongoConfiguration(this IServiceProvider services)
    {
        await services.ExecuteWithScoped<MongoConfigurationManager>((manager) => manager.Apply());
    }
}