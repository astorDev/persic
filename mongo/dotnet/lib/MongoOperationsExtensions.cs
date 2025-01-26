using MongoDB.Bson;
using MongoDB.Driver;

namespace Persic;

public static class MongoOperationsExtensions
{
    public static async Task<ReplaceOneResult> Put<TDocument>(this IMongoCollection<TDocument> collection, TDocument document) 
        where TDocument : IMongoRecord<string>
    {
        return await collection.ReplaceOneAsync(
            (x) => x.Id == document.Id,
            document,
            options: new ReplaceOptions() { IsUpsert = true }
        );
    }

    public static async Task<BsonDocument> Ping(this IMongoDatabase database)
    {
        return await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    }
}

public interface IMongoRecord<TKey>
{
    TKey Id { get; }
}