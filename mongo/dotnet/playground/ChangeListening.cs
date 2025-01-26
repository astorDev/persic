using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Persic;

namespace Playground.Mongo;

[TestClass]
public sealed class ChangeListening
{
    [TestMethod]
    public async Task Connecting()
    {
        var collection = GetCollection();
        var pong = await collection.Database.Ping();
        Console.WriteLine(pong);
    }

    [TestMethod]
    [ExpectedException(typeof(MongoCommandException))]
    public async Task Naive()
    {
        var collection = this.GetCollection();
        await ExecuteWatching(collection);
    }

    [TestMethod]
    public async Task Valid()
    {
        var collection = this.GetCollectionWithReplicaSet();
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
    
    public IMongoCollection<Robot> GetCollectionWithReplicaSet()
    {
        var client = new MongoClient("mongodb://localhost:27017/?replicaSet=rs0&serverSelectionTimeoutMS=2000");
        var database = client.GetDatabase("persic-playground");
        return database.GetCollection<Robot>("robots");
    }
    
    public IMongoCollection<Robot> GetCollection()
    {
        var client = new MongoClient("mongodb://localhost:27019/");
        var database = client.GetDatabase("persic-playground");
        return database.GetCollection<Robot>("robots");
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