using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Persic;

public static class MongoDependencyInjection {
    public static CollectionBuilder AddMongo(
        this IServiceCollection services, 
        Func<IServiceProvider, IMongoClient> clientFactory, 
        string databaseName
        )
    {
        services.AddSingleton(clientFactory);
        services.AddSingleton(sp => {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        return new CollectionBuilder(services);
    }

    public static CollectionBuilder AddMongo(
        this IServiceCollection services,
        string connectionString,
        string databaseName
    ) => services.AddMongo(sp => new MongoClient(connectionString), databaseName);

    public class CollectionBuilder(IServiceCollection services) {
        public CollectionBuilder AddCollection<T>(string name) {
            services.AddMongoCollection<T>(name);
            return this;
        }
    }

    public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string name)
    {
        services.AddSingleton((sp) => {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<T>(name);
        });

        return services;
    }
}
