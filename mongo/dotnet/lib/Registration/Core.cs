using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Persic;

public static class MongoDependencyInjection {
    public static MongoRegistrationBuilder AddMongo(
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

        services.AddScoped<MongoConfigurationManager>();

        return new MongoRegistrationBuilder(services);
    }

    public static MongoRegistrationBuilder AddMongo(
        this IServiceCollection services,
        string connectionString,
        string databaseName
    ) => services.AddMongo(sp => new MongoClient(connectionString), databaseName);

    

    public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string name)
    {
        services.AddSingleton((sp) => {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<T>(name);
        });

        return services;
    }

    public static IServiceCollection AddOpenMongoCollections(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IMongoCollection<>) , typeof(MongoCollection<>));

        return services;
    }
}