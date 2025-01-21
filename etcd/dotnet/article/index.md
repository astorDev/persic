# etcd with .NET

> Using etcd key-value store in C#: writing, reading key-value pairs, listening for changes, and executing a transaction.

![Yet another Key-Value store with C#]()

Etcd is yet another key-value store. However, unlike Redis the database is strongly consistent, making it a better choice for storing critical data of a distributed system. 

In this article, we will deploy and play around with the database using C# and .NET. Without further ado, let's get going! 

## Deploying Etcd Locally via Docker

First of all, we'll need to actually have an `etcd` instance to connect to. Gladly, deploying it via docker is pretty trivial. We'll just need to allow connecting to the instance without any authentication, map the storage folder to a persistent volume, and map the default `2379` port. Here's what our `compose.yml` file will look like:

```yaml
services:
  etcd:
    image: bitnami/etcd:latest
    environment:
      - ALLOW_NONE_AUTHENTICATION=yes
    ports:
      - "2379:2379"
    volumes:
      - etcd-data:/bitnami/etcd

volumes:
  etcd-data:
```

Now, after deploying the container with:

```sh
docker compose up -d
```

We are ready to go! Now let's jump to the C# code.

## Performing Basic Key-Value Operations

First thing first, we'll need to add a package, providing us with a client for our database. Here's the command that does just that:

> This assumes you are in a folder containing a .NET project. The easiest way to create one is by running `dotnet new console`.

```sh
dotnet add package dotnet-etcd
```

Now, let's connect to our database. We'll also need to explicitly specify that we will skip authorization. Here's the code:

```csharp
using dotnet_etcd;
using Grpc.Core;

// ...

EtcdClient etcdClient = new(
    "http://localhost:2379",
    configureChannelOptions: options => 
        options.Credentials = ChannelCredentials.Insecure
);
```

Finally, let's write and read a key-value pair:

```csharp
await etcdClient.PutAsync("name", "Egor");
var received = await etcdClient.GetValAsync("name");
Console.WriteLine($"Received. name = {received}");
```

After running the code, we should get this in the console:

```text
Received. name = Egor
```

This is practically everything we need to know about etcd fundamentals. Now, let's jump to some advanced stuff!

## Listening for Changes using Etcd `WatchRequest`

What makes `etcd` stand apart is its ability to watch changes for a particular key or a set of keys. We can do it by using the `WatchAsync` method of our `EtcdClient`. 

The `Task` returned by the method seems to run indefinitely (for the duration of the watching). So we won't `await` it, but instead just write it to a discard (`_`). On receiving a change we'll print the information in the console, along with the details of the events. Finally, to make time for the event to occur and be logged we'll `Delay` our thread just for 100 milliseconds. Here's the code:

```csharp
using Etcdserverpb;

// ...

_ = etcdClient.WatchAsync(
    "dog",
    (WatchEvent[] response) =>
    {
        Console.WriteLine("received watch response");
        
        foreach (var watchEvent in response)
        {
            Console.WriteLine($"Received event: {watchEvent.Key} -> {watchEvent.Value}. ({watchEvent.Type})");
        }
    }
);

await etcdClient.PutAsync("dog", "sits");
await etcdClient.PutAsync("dog", "runs");

await Task.Delay(100);
```

After running the code we'll get:

```text
received watch response
received watch response
Received event: dog -> sits. (Put)
received watch response
Received event: dog -> runs. (Put)
```

Apparently, a subscription for events also triggers the watch callback with an empty `WatchEvent` array. That's why we have two `received watch response` messages in the beginning. The change-listening mechanics of the `etcd` are cool, yet this is not all. Let's do just one more advanced thing!

## Atomic Write using Etcd `TxnRequest`

Etcd also allows us to atomically write multiple keys, using a transactions mechanism. This time we'll need a slightly more verbose syntax to achieve our goal, but it's still pretty easy to comprehend. Here's how we can write two animal sounds in a single Transaction:

```csharp
using Google.Protobuf;

// ...

var transaction = new TxnRequest();
        
transaction.Success.AddRange(new []
{
    new RequestOp { 
        RequestPut = new()
        {
            Key = ByteString.CopyFromUtf8("animals/cow"),
            Value = ByteString.CopyFromUtf8("moo")
        } 
    },
    new RequestOp { 
        RequestPut = new()
        {
            Key = ByteString.CopyFromUtf8("animals/chicken"),
            Value = ByteString.CopyFromUtf8("coo")
        } 
    }
});

await etcdClient.TransactionAsync(transaction);
        
var cow = await etcdClient.GetValAsync("animals/cow");
var chicken = await etcdClient.GetValAsync("animals/chicken");
        
Console.WriteLine($"cow = {cow}");
Console.WriteLine($"chicken = {chicken}");
```

This time, instead of just running our code, let's do something more fun! How about combining the transactions approach with change listening?

Let's listen to all changes in the animals "folder". etcd treats keys as byte sequences and compares them lexicographically. That means that in order to get all `animals/` we'll have to specify a range starting from `animals/` and ending right after it (at `animals0`, where `0` is the next character after `/` ). Here's the listener code:

```csharp
var request = new WatchRequest()
{
    CreateRequest = new()
    {
        Key = ByteString.CopyFromUtf8("animals/"),
        RangeEnd = ByteString.CopyFromUtf8("animals0")
    }
};
        
_ = etcdClient.WatchAsync(
    request,
    (WatchEvent[] response) =>
    {
        Console.WriteLine("received watch response");

        foreach (var watchEvent in response)
        {
            Console.WriteLine($"Received event: {watchEvent.Key} -> {watchEvent.Value}. ({watchEvent.Type})");
        }
    }
);
```

Combining the two snippets and running them together will result in the following output

```text
received watch response
received watch response
Received event: dog -> sits. (Put)
received watch response
Received event: dog -> runs. (Put)
received watch response
received watch response
Received event: animals/cow -> moo. (Put)
Received event: animals/chicken -> coo. (Put)
cow = moo
chicken = coo
```

As you may see, the transaction was sent to our watcher as a single response, containing two events. And this is the most advanced thing I have for you to see. Let's recap and call it a day!

## Recap

Here's what we have done in the article:

- Deployed local instance of etcd.
- Performed basic operations via C# code.
- Implemented change listening via `WatchRequest`
- Performed complex updates via `TxnRequest`.

You can find the source code from the article [in this GitHub repository](https://github.com/astorDev/persic/blob/main/etcd/dotnet/playground/JumpStart.cs). Don't hesitate to give the repository a star ⭐! Don't hesitate to clap for this article either! 😉
