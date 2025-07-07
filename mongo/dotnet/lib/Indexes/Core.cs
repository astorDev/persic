using MongoDB.Driver;

namespace Persic;

public interface ICreateIndexModelProvider<T>
{
    CreateIndexModel<T> IndexModel { get; }
}

public static partial class MongoIndexManager
{
    public static async Task Create<T>(
        this IMongoIndexManager<T> mongoIndexManager,
        ICreateIndexModelProvider<T> indexModelProvider,
        CancellationToken cancellationToken = default
    )
    {
        await mongoIndexManager.CreateOneAsync(indexModelProvider.IndexModel, cancellationToken: cancellationToken);
    }
}