# etcd with .NET

> Using etcd key-value store in C# - writing, reading key-value pairs, listening for changes, and executing a transaction.

![Yet another Key-Value store with C#]()

Etcd is yet another key-value store. However, unlike Redis the database is strongly consistent, making it a better choice for storing critical data of a distributed system. 

In this article, we will deploy and play around with the database using C# and .NET. Without further ado, let's get going! 

## Recap

Here's what we have done in the article:

- Deployed local instance of etcd.
- Performed basic operations via C# code.
- Implemented change listening via `WatchRequest`
- Performed complex updates via `TxnRequest`.

You can find the source code from the article [in this GitHub repository](https://github.com/astorDev/persic/blob/main/etcd/dotnet/playground/JumpStart.cs). Don't hesitate to give the repository a star ⭐! Don't hesitate to clap for this article either! 😉
