using System.Linq.Expressions;
using MongoDB.Driver;

namespace Persic;

public record TimeToLive<T>(Expression<Func<T, object>> Field, TimeSpan? ExpireAfter = null)
    : ICreateIndexModelProvider<T>
{
    public CreateIndexModel<T> IndexModel =>
        new(
            Builders<T>.IndexKeys.Ascending(Field),
            new() { ExpireAfter = ExpireAfter ?? TimeSpan.Zero }
        );

}

public static partial class MongoIndexManager
{
    public static async Task CreateTimeToLive<T>(this IMongoIndexManager<T> mongoIndexManager, Expression<Func<T, object>> field, TimeSpan? expireAfter = null, CancellationToken cancellationToken = default)
    {
        var indexModelProvider = new TimeToLive<T>(field, expireAfter);
        await mongoIndexManager.Create(indexModelProvider, cancellationToken);
    }
}