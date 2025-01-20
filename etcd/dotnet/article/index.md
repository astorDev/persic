# etcd with .NET

> Using etcd key-value store in C#: writing, reading key-value pairs, listening for changes, and executing a transaction.

![Yet another Key-Value store with C#]()

Etcd is yet another key-value store. However, unlike Redis the database is strongly consistent, making it a better choice for storing critical data of a distributed system. 

In this article, we will deploy and play around with the database using C# and .NET. Without further ado, let's get going! 

## Deploying Etcd Locally via Docker

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

## Performing Basic Key Value Operations

```sh
dotnet add package dotnet-etcd
```

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

```csharp
await etcdClient.PutAsync("name", "Egor");
var received = await etcdClient.GetValAsync("name");
Console.WriteLine($"Received. name = {received}");
```

```text
Received. name = Egor
```

## Listening for Changes using Etcd `WatchRequest`

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

```text
received watch response
received watch response
Received event: dog -> sits. (Put)
received watch response
Received event: dog -> runs. (Put)
```

## Atomic Write using Etcd `TxnRequest`

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

## Recap

Here's what we have done in the article:

- Deployed local instance of etcd.
- Performed basic operations via C# code.
- Implemented change listening via `WatchRequest`
- Performed complex updates via `TxnRequest`.

You can find the source code from the article [in this GitHub repository](https://github.com/astorDev/persic/blob/main/etcd/dotnet/playground/JumpStart.cs). Don't hesitate to give the repository a star ⭐! Don't hesitate to clap for this article either! 😉
