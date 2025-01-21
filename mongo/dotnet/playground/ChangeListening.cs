using MongoDB.Driver;

namespace Playground.Mongo;

[TestClass]
public sealed class ChangeListening
{
    [TestMethod]
    public async Task Basic()
    {
        var collection = this.GetCollection();
        
        await collection.Put(new(Guid.NewGuid().ToString(), 11));
        
        _ = collection.RunWatching((c) =>
        {
            Console.WriteLine($"{c.OperationType} -> {c.FullDocument}");
        });
        
        await collection.Put(new(Guid.NewGuid().ToString(), 29));
        await collection.Put(new(Guid.NewGuid().ToString(), 27));
        await Task.Delay(100);
    }
    

    public IMongoCollection<Person> GetCollection()
    {
        var client = new MongoClient("mongodb://localhost:27017/?replicaSet=my-mongo-set");
        var database = client.GetDatabase("persic-playground");
        return database.GetCollection<Person>("people");
    }
}

public record Person(string Id, int Age) : MongoRecord(Id);

public record MongoRecord(string Id);

public static class MongoCollectionExtensions
{
    public static async Task<ReplaceOneResult> Put<TDocument>(this IMongoCollection<TDocument> collection, TDocument document) where TDocument : MongoRecord
    {
        return await collection.ReplaceOneAsync(
            (x) => x.Id == document.Id,
            document,
            options: new ReplaceOptions() { IsUpsert = true }
        );
    }
    
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