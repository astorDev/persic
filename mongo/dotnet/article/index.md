# MongoDB Changes Watching using C#

> Reacting to MongoDB collection changes in .NET by using `WatchAsync` and `IChangeStreamCursor`. Plus, deploying MongoDB instance with replica set enabled locally via docker-compose.

![Listening to Mongo using .NET. Not Long-Polling]()

MongoDB is becoming an increasingly popular database option in modern systems, including those written in .NET. However, one of its advanced features - change listening, is frequently overlooked. In this article I'll shed light on that functionality, providing a ready-to-go example of listening to changes in a MongoDB collection using C#.

## Deploying and Connection to MongoDB via C#: Naive Version

`compose.yml`

```yml
services:
  simple:
    image: mongo
    ports:
      - 27019:27017
```

> This assumes you are in a folder containing a .NET project. The easiest way to create one is by running `dotnet new console`.

```sh
dotnet add package MongoDB.Driver
```


```csharp
public IMongoCollection<Robot> GetCollection()
{
    var client = new MongoClient("mongodb://localhost:27019/");
    var database = client.GetDatabase("persic-playground");
    return database.GetCollection<Robot>("robots");
}
```

```sh
dotnet add package Persic.Mongo
```

```csharp
var database = GetDatabase();
var pong = await database.Ping();
Console.WriteLine(pong);
```

```json
{ "ok" : 1.0 }
```

## Implementing Change Listening

```csharp
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
```

```csharp
public record Robot(string Id, int Type) : IMongoRecord<string>;
```

```csharp
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
```

```csharp
var collection = GetDatabase().GetCollection<Robot>("robots");
await ExecuteWatching(collection);
```

```text
Command aggregate failed: The $changeStream stage is only supported on replica sets.
```

## Deploying MongoDB with replica set via Docker Compose

`init.js`

```js
rs.initiate({
    _id: "rs0",
    members: [
        { _id: 0, host: "localhost:27017" }
    ]
});
```

```yaml
services:
  ## ...

  with-replica-set:
    image: mongo
    ports:
      - 27017:27017
    command: mongod --replSet rs0
    volumes:
      - ./init.js:/docker-entrypoint-initdb.d/init.js
```

## Connecting to Mongo with replicaSet specification.

```csharp
public IMongoDatabase GetDatabaseWithReplicaSet()
{
    var client = new MongoClient("mongodb://localhost:27017/?replicaSet=rs0");
    return client.GetDatabase("persic-playground");
}
```

```csharp
var collection = GetDatabaseWithReplicaSet().GetCollection<Robot>("robots");
await ExecuteWatching(collection);
```

```text
Insert -> Robot { Id = b9116c9e-f0c1-43e8-b35a-0e706dd474c4, Type = 29 }
Insert -> Robot { Id = d2f1c41f-1469-4bc0-bd06-5a4598df46b9, Type = 27 }
```

## Wrapping Up!

After deploying MongoDB with replica set enabled we were able to listen for changes in a collection using C# code. You can find the source code for this article [here on GitHub](https://github.com/astorDev/persic/tree/main/mongo). The repository holds tools and playgrounds for various databases, beyond Mongo. Don't hesitate to give [it](https://github.com/astorDev/persic) a star! ⭐ Don't hesitate to clap for this article either! 😉
