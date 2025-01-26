# MongoDB Changes Watching using C#

> Reacting to MongoDB collection changes in .NET by using `WatchAsync` and `IChangeStreamCursor`. Plus, deploying MongoDB instance with replica set enabled locally via docker compose.

![Listening to Mongo using .NET. Not Long-Polling]()

MongoDB becomes increasingly popular option for a database in modern systems, including those written in .NET. However, one of the advanced feature of it - change listening, is frequently overlooked. In this article I'll shed a light of that functionality, providing a ready to go example on how to listen to changes in a MongoDB collection using C#.

## Wrapping Up!

After deploying MongoDB with replica set enable we were able to listen for changes in a collection using C# code. You can find the source code for this article [here in the GitHub](https://github.com/astorDev/persic/tree/main/mongo). The repository holds tools and playgrounds for various databases, beyond Mongo. Don't hesitate to give [it](https://github.com/astorDev/persic) a star! ⭐ Don't hesitate to clap for this article either! 😉