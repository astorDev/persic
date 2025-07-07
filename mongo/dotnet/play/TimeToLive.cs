using MongoDB.Driver;

namespace Persic.Mongo.Playground;

[TestClass]
public class TimeToLive
{
    [TestMethod]
    public async Task Message()
    {
        var services = new ServiceCollection()
            .AddMongo("mongodb://localhost:27017", "ttl-play")
            .AddCollection<ShortLivedMessage, ShortLivedMessage.Configuration>("messages")
            .Services
            .BuildServiceProvider();

        await services.ApplyMongoConfiguration();

        var collection = services.GetRequiredService<IMongoCollection<ShortLivedMessage>>();

        var message = new ShortLivedMessage(
            Id: "msg-1",
            Text: "Hello, world!",
            CreatedAt: DateTime.UtcNow,
            ExpiresAt: DateTime.UtcNow.AddSeconds(2)
        );

        await collection.InsertOneAsync(message);

        var inserted = await collection.Search("msg-1");

        inserted.ShouldNotBeNull();

        var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(63));

        while (!timeout.IsCancellationRequested)
        {
            var found = await collection.Search("msg-1");
            if (found != null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), timeout.Token);
            }
            else
            {
                break;
            }
        }

        var afterDelay = await collection.Search("msg-1");

        afterDelay.ShouldBeNull();
    }
}

public record ShortLivedMessage(
    string Id,
    string Text,
    DateTime CreatedAt,
    DateTime ExpiresAt
) : IMongoRecord<string>
{
    public class Configuration(IMongoCollection<ShortLivedMessage> collection) : IMongoCollectionConfiguration
    {
        public async Task Apply()
        {
            await collection.Indexes.CreateTimeToLive(x => x.ExpiresAt);
        }
    }
}