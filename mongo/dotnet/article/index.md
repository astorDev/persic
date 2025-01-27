# MongoDB Changes Watching using C#

> Reacting to MongoDB collection changes in .NET by using `WatchAsync` and `IChangeStreamCursor`. Plus, deploying MongoDB instance with replica set enabled locally via docker-compose.

![Listening to Mongo using .NET. Not Long-Polling]()

MongoDB is becoming an increasingly popular database option in modern systems, including those written in .NET. However, one of its advanced features - change listening, is frequently overlooked. In this article I'll shed light on that functionality, providing a ready-to-go example of listening to changes in a MongoDB collection using C#.

## Deploying and Connection to MongoDB via C#: Naive Version

First thing first, let's set up our environment. At the start, We'll need to deploy Mongo locally. Here's `compose.yml` that does just that:

```yml
services:
  simple:
    image: mongo
    ports:
      - 27019:27017
```

Next, we'll need to connect to the database via C# code. Of course, we'll need a client to do that. To install it we can run the script below:

> This assumes you are in a folder containing a .NET project. The easiest way to create one is by running `dotnet new console`.

```sh
dotnet add package MongoDB.Driver
```

Now, let's get a database to connect to:

```csharp
public IMongoDatabase GetDatabase()
{
    var client = new MongoClient("mongodb://localhost:27019/");
    return client.GetDatabase("persic-playground");
}
```

To test the connection we'll use a ping command. Although the command is built in MongoDB there's no easy way to call it using the official driver. Gladly, there's a library that provides a `Ping` extension method along with other useful MongoDB utils. Let's install it:

```sh
dotnet add package Persic.Mongo
```

Now we can use the extension method and print its result to the console. Here's the code:

```csharp
var database = GetDatabase();
var pong = await database.Ping();
Console.WriteLine(pong);
```

After running the code, in the console we should see JSON resembling this:

```json
{ "ok" : 1.0 }
```

The setup is done! Let's move to the interesting part

## Implementing Change Listening

By running `WatchAsync` on a Mongo collection we can get an `IChangeStreamCursor`. Iterating the cursor via `MoveNextAsync` we should be able to rotate our `ChangeStream.Current` to a newly updated event. Let's assemble this into a single method, accepting `Action<ChangeStreamDocument<TDocument>>` for a change handler. Here's what our code might look like:

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

To properly work with a Mongo collection we'll need to have an underlying model type. Let's define a simple `Robot` class, just to use something specific

> We will also implement `IMongoRecord<string>` from the `Persic.Mongo` library. That will allow us to use the `Put` method from the library

```csharp
public record Robot(string Id, int Type) : IMongoRecord<string>;
```

Our main process will start by triggering the watch task. We will **not** await it, though. The task under normal circumstances is supposed to run forever, so we will not be able to do something else if we would await it. Instead, we will save the task as a variable, run an example operation on our collection. Something like this:

```csharp
var watchTask = collection.RunWatching((c) =>
{
    Console.WriteLine($"{c.OperationType} -> {c.FullDocument}");
});
    
await collection.Put(new(Guid.NewGuid().ToString(), 29));
await collection.Put(new(Guid.NewGuid().ToString(), 27));
```

But now we won't notice if an exception will happen during the watching process. Let's fix it, by checking if our `watchTask` is completed (failed). If it did we will let it throw the exception by awaiting it. Here's the code:

```csharp
if (watchTask.IsCompleted) await watchTask;
```

We also may need some time before the `ChangeStream` figures out that something has happened. Let's give it at least one-tenth of a second to do so. Like this:

```csharp
await Task.Delay(100);
```

Assembling all of that together we'll have a method looking like this:

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

And finally, let's assemble our parts together. Our first attempt code should look something like this:

```csharp
var collection = GetDatabase().GetCollection<Robot>("robots");
await ExecuteWatching(collection);
```

Unfortunately, running this code will give us an error like this:

```text
Command aggregate failed: The $changeStream stage is only supported on replica sets.
```

Well, this is harder than we might have expected. However, it's clear what we'll need to do. We'll need to enable replica sets!

## Deploying MongoDB with a replica set via Docker Compose

Unfortunately, deploying Mongo with a replica set is not that trivial either. The main problem is the fact that we have to run a script inside a Mongo shell to initiate it, which is not something trivial to do in Docker. 

Gladly, mongo containers seem to automatically run the script inside the `docker-entrypoint-initdb.d` folder, which is exactly what we are looking for. Let's define such a script it a file called `init.js` in the same folder as our compose file.

```js
rs.initiate({
    _id: "rs0",
    members: [
        { _id: 0, host: "localhost:27017" }
    ]
});
```

Now, let's create a mongo instance with a replica set enabled. We'll need to do two things to achieve that:

1. Set command to run with `--replSet` flag
2. Supply our `init.js` to the containers' startup scripts.

Here's what the docker compose might look like:

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

After deploying the instance via `docker compose up -d` we should get a MongoDB with a replica set running on port `27017` on our local machine. Let's use it now

## Connecting to Mongo with replicaSet specification.

To use a replica set we also need to specify it explicitly in our connection string. Let's create a new method for connecting to our database:

```csharp
public IMongoDatabase GetDatabaseWithReplicaSet()
{
    var client = new MongoClient("mongodb://localhost:27017/?replicaSet=rs0");
    return client.GetDatabase("persic-playground");
}
```

Let's now use it to create our second attempt code:

```csharp
var collection = GetDatabaseWithReplicaSet().GetCollection<Robot>("robots");
await ExecuteWatching(collection);
```

Now, by running it we should get output from the function we supplied into our `RunWatching` method. Let me remind you of the code we've used:

```csharp
var watchTask = collection.RunWatching((c) =>
{
    Console.WriteLine($"{c.OperationType} -> {c.FullDocument}");
});
```

And here's the output we received!

```text
Insert -> Robot { Id = b9116c9e-f0c1-43e8-b35a-0e706dd474c4, Type = 29 }
Insert -> Robot { Id = d2f1c41f-1469-4bc0-bd06-5a4598df46b9, Type = 27 }
```

And that's how you listen to MongoDb changes using C# code.

## Wrapping Up!

After deploying MongoDB with replica set enabled we were able to listen for changes in a collection using C# code. You can find the source code for this article [here on GitHub](https://github.com/astorDev/persic/tree/main/mongo). The repository holds tools and playgrounds for various databases, beyond Mongo. Don't hesitate to give [it](https://github.com/astorDev/persic) a star! ⭐ Don't hesitate to clap for this article either! 😉
