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
}

public interface IMongoRecord<TKey>
{
    TKey Id { get; }
}