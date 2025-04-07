# Integrating PostgreSQL with .NET 9 Using EF Core: A Step-by-Step Guide

> A Practical Example of Connecting an ASP .NET Core App to PostgreSQL using Entity Framework

PostgreSQL is the most popular database out there, according to the [latest StackOverflow survey](https://survey.stackoverflow.co/2024/technology#1-databases). And, of course, EF Core, as a versatile ORM plays nicely with it. Still to integrate those two one will need to make a few steps with a few caveats along the way. In this article, we will go through those steps together, implementing a couple of helper methods to make the integration even simpler in the future.

> If you just want to use the simplified PostgreSQL connection, jump straight to the end of this article to the [TLDR;](#tldr) section.

## TLDR;

In this article, we've implemented a helper method for seamless registration of a PostgreSQL database in a .NET application. Instead of recreating the method from scratch, you can use the `Persic.EF` package:

```sh
dotnet add package Persic.EF.Postgres
```

With the package installed, you will be able to attach your database with just a single line of code:

```csharp
builder.Services.AddPostgres<Db>();
```

Of course, you would need to deploy the database first. Here's a simple `compose.yml` for that:

```yaml
services:
  postgres:
    image: postgres
    environment:
      POSTGRES_DB: playground
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - 5432:5432
```

Finally, don't forget to set your connection string to the `ConnectionStrings:Postgres` configuration value. My [recommendation](https://medium.com/@vosarat1995/net-configuration-architecture-getting-started-87526b9fbc68) is to utilize `launchSettings.json` for that:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5267",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ConnectionStrings:Postgres" : "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=playground"
      }
    }
  }
}
```

Here's a simple code snippet to test your connection:

```csharp
await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<Db>();
var canConnect = await db.Database.CanConnectAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);
```

This wraps up our PostgreSQL integration journey. This article, along with the `Persic.EF.Postgres` package, is part of a project called [persic](https://github.com/astorDev/persic), containing various DB-related tooling. Check it out on [GitHub](https://github.com/astorDev/persic) and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉
