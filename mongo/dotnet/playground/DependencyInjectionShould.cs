using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Persic;
using Playground.Mongo;

[TestClass]
public class DependencyInjectionShould {
    [TestMethod]
    public void ResolveCollections() {
        var services = new ServiceCollection();

        services.AddMongo(sp => new MongoClient(), "test")
            .AddCollection<Robot>("robots")
            .AddCollection<Robot>("orders");

        var provider = services.BuildServiceProvider();

        var robotCollection = provider.GetRequiredService<IMongoCollection<Robot>>();
    }

    [TestMethod]
    public async Task ResolvePingableDatabase() {
        var services = new ServiceCollection();

        services.AddMongo("mongodb://localhost:27017", "persic-di-playground");

        var database = services.BuildServiceProvider().GetRequiredService<IMongoDatabase>();

        var pingResult = await database.Ping();
        Console.WriteLine(pingResult);
    }
}

public record Order(string Id, string ProductId);