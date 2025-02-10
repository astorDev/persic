using MongoDB.Driver;
using Persic;

namespace Playground.Mongo;

[TestClass]
public sealed class ChangeListening
{
    [TestMethod]
    public async Task Connecting()
    {
        var database = GetDatabase();
        var pong = await database.Ping();
        Console.WriteLine(pong);
    }

    [TestMethod]
    [ExpectedException(typeof(MongoCommandException))]
    public async Task Naive()
    {
        var collection = GetDatabase().GetCollection<Robot>("robots");
        await ExecuteWatching(collection);
    }

    [TestMethod]
    public async Task Valid()
    {
        var collection = GetDatabaseWithReplicaSet().GetCollection<Robot>("robots");
        await ExecuteWatching(collection);
    }

    [TestMethod]
    public async Task Imaged()
    {
        var collection = GetImagedDatabase().GetCollection<Robot>("robots");
        await ExecuteWatching(collection);
    }

    public async Task ExecuteWatching(IMongoCollection<Robot> collection)
    {
        var watchTask = collection.RunWatching((c) =>
        {
            Console.WriteLine($"{c.OperationType} -> {c.FullDocument}");
        });
        
        await collection.Put(new(Guid.NewGuid().ToString(), 29));
        await collection.Put(new(Guid.NewGuid().ToString(), 27));
        
        if (watchTask.IsCompleted) await watchTask;
        await Task.Delay(100);
    }
    
    public IMongoDatabase GetDatabaseWithReplicaSet()
    {
        var client = new MongoClient("mongodb://localhost:27017/?replicaSet=rs0");
        return client.GetDatabase("persic-playground");
    }
    
    public IMongoDatabase GetDatabase()
    {
        var client = new MongoClient("mongodb://localhost:27019/");
        return client.GetDatabase("persic-playground");
    }

    public IMongoDatabase GetImagedDatabase()
    {
        var client = new MongoClient("mongodb://localhost:27018/?replicaSet=rs0");
        return client.GetDatabase("persic-playground");
    }
}

public record Robot(string Id, int Type) : IMongoRecord<string>;

public static class MongoCollectionExtensions
{
    public static async Task RunWatching<TDocument>(
        this IMongoCollection<TDocument> collection, 
        Action<ChangeStreamDocument<TDocument>> changeHandler)
    {
        using var changeStream = await collection.WatchAsync();
        while (await changeStream.MoveNextAsync())
        {
            foreach (var change in changeStream.Current)
            {
                changeHandler(change);
            }
        }
    }
}