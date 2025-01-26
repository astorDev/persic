using MongoDB.Driver;
using Persic;

namespace Playground.Mongo;

[TestClass]
public sealed class ChangeListening
{
    [TestMethod]
    public async Task Basic()
    {
        var collection = this.GetCollectionWithReplicaSet();
        
        _ = collection.RunWatching((c) =>
        {
            Console.WriteLine($"{c.OperationType} -> {c.FullDocument}");
        });
        
        await collection.Put(new(Guid.NewGuid().ToString(), 29));
        await collection.Put(new(Guid.NewGuid().ToString(), 27));
        await Task.Delay(100);
    }
    
    public IMongoCollection<Robot> GetCollectionWithReplicaSet()
    {
        var client = new MongoClient("mongodb://localhost:27017/?replicaSet=rs0");
        var database = client.GetDatabase("persic-playground");
        return database.GetCollection<Robot>("people");
    }
    
    public IMongoCollection<Robot> GetCollection()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("persic-playground");
        return database.GetCollection<Robot>("people");
    }
}

public record Robot(string Id, int Type) : IMongoRecord<string>;

public static class MongoCollectionExtensions
{
    public static async Task RunWatching<TDocument>(
        this IMongoCollection<TDocument> collection, 
        Action<ChangeStreamDocument<TDocument>> changeHandler)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}