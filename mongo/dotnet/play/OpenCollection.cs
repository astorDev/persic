using MongoDB.Driver;

namespace Persic.Mongo.Playground;

[TestClass]
public class Registration
{
    [TestMethod]
    public async Task OpenCollections()
    {
        var services = new ServiceCollection()
            .AddMongo("mongodb://localhost:27017", "open-collections")
            .AddOpenCollections()
            .Services
            .BuildServiceProvider();

        var x = services.GetRequiredService<IMongoCollection<OpenlyRegisteredRecord>>();
        x.InsertOne(new OpenlyRegisteredRecord { Id = Guid.NewGuid().ToString(), Data = "Sample data" });

        x.ShouldNotBeNull();
    }
}

public class OpenlyRegisteredRecord
{
    public required string Id { get; set; }
    public required string Data { get; set; }
}