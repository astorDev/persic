# Postgres Full Text Search with EF Core 9

> No Elasticsearch needed.

Text search is something relational databases have historically struggled with. This limitation even led to a rise of dedicated search tools, like Elasticsearch.

However, since Postgres 8.3, we got a thing called `tsvector`, providing us a powerful way to query our data straight from the relational database. In this article, we are going to play around with the text search in PostgreSQL and build a few helpers using .NET 9 and Entity Framework Core.

> Or jump straight to the [TLDR](#tldr) at the end of this article for a short reference.

## Setting Up Our Database

> `dotnet new console` or `dotnet new mstest`

```sh
dotnet add package Persic.EF.Postgres
```

```csharp
services:
  postgres:
    image: postgres
    environment:
      POSTGRES_DB: postgres_search
      POSTGRES_USER: postgres_search
      POSTGRES_PASSWORD: postgres_search
    ports:
      - 5631:5432
```

```csharp
public class Db : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UsePostgres("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search");
    }
}
```

```csharp
using var db = new Db();
await db.Database.OpenConnectionAsync();
```

## Making Our First TsVector Query



```csharp
public class DbRecord
{
    public required string Id { get; set; }
    public required string SearchTerms { get; set; }

    public static DbRecord New(string searchTerms) => new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = searchTerms
    };
}

public class Db : DbContext
{
    public DbSet<DbRecord> Records { get; set; }

    // ..
}
```

```csharp
public static class Seeds
{
    public const string HelloWorld = "Hello World!";
    public const string HelloJohn = "Hello John!";
    public const string ByeJohn = "Bye, John!";
    public const string Bye = "Bye!";
    public const string JackBlack = "Jack Black";
    public const string JackCustome = "Jack Custome";
    public const string JackBrown = "Jack Brown";

    public static readonly string[] All =
    [
        HelloWorld,
        HelloJohn,
        ByeJohn,
        Bye,
        JackBlack,
        JackCustome,
        JackBrown
    ];

    public static T[] As<T>(Func<string, T> factory) => [.. All.Select(factory)];
}
```

```csharp
public static async Task<Db> Seeded()
{
    var db = new Db();
    var seeds = Seeds.As(DbRecord.New);
    await db.EnsureRecreated(x => x.Records.AddRange(seeds));
    return db;
}
```

```csharp
using var db = await Db.Seeded();

var result = await db.Records
    .Where(x =>
        EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("Hello World")
    )
    .ToListAsync();
```

## Using Advanced Text Search Patterns

## Improving Performance With TsVector Columns

## TLDR

In this article, we've experimented with search functionality in PostgreSQL using Entity Framework. Along the way, we've made a few helpers. Instead of recreating those helpers, you can install a dedicated NuGet package like this:

```sh
dotnet add package Persic.EF.Postgres.Search
```

With the package in place, you should be able to make your model suitable for search, implementing `IRecordWithSearchVector` like this:

```csharp
public class DbRecord : IRecordWithSearchVector
{
    public required string Id { get; set; }
    public required string SearchTerms { get; set; }
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}
```

Set up automatic conversion from your text field along with the index for the search:

```csharp
modelBuilder.Entity<DbRecord>()
            .HasIndexedSearchVectorGeneratedFrom(
                p => p.SearchTerms
            );
```

And finally, query your record using a simple extension method:

```csharp
var result = await db.Records
            .WhereSearchVectorMatches("jack & b:*")
            .ToListAsync();
```

The package, as well as this article, is part of a project called [persic](https://github.com/astorDev/persic), containing various DB-related tooling. Check it out on [GitHub](https://github.com/astorDev/persic), and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉
