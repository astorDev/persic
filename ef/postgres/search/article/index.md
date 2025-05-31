# Postgres Full Text Search with EF Core 9

> No Elasticsearch needed.

![](thumb.png)

Text search is something relational databases have historically struggled with. This limitation even led to a rise of dedicated search tools, like Elasticsearch.

However, since Postgres 8.3, we got a thing called `tsvector`, providing us a powerful way to query our data straight from the relational database. In this article, we are going to play around with the text search in PostgreSQL and build a few helpers using .NET 9 and Entity Framework Core.

> Or jump straight to the [TLDR](#tldr) at the end of this article for a short reference.

## Setting Up Our Database

To start, let’s set up a local instance of PostgreSQL. Here’s a simple `compose.yml` that does just that:

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

Of course, we will also need a .NET project. For this article, a simple console app will be just fine. You can create it with the following command:

```sh
dotnet new console
```

In [this article](https://medium.com/@vosarat1995/integrating-postgresql-with-net-9-using-ef-core-a-step-by-step-guide-a773768777f2) about integrating PostgreSQL and .NET we've discovered a NuGet package greatly simplifying the setup. Let's install it:

```sh
dotnet add package Persic.EF.Postgres
```

Finally, let's define our database. For our model, we'll need only `SearchTerms` – a string against which we will perform searches. Here's the code:

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UsePostgres("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search");
    }
}
```

To ensure our setup works, let's open a connection to our database:

```csharp
using var db = new Db();
await db.Database.OpenConnectionAsync();
```

Hopefully, you were able to connect successfully. With our setup done, let's move to the interesting part: performing the search.

## Making Our First TsVector Query

Before we start querying our database, we need some data in there. We'll create relatively similar data, demonstrating various uses of the text search. Let's create a helper class just for that:

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

Let's also create a helper method that spins up our database with the data above. We'll use `EnsureRecreated` helper method from the `Persic.EF` package:

```csharp
public static async Task<Db> Seeded()
{
    var db = new Db();
    var seeds = Seeds.As(DbRecord.New);
    await db.EnsureRecreated(x => x.Records.AddRange(seeds));
    return db;
}
```

Finally, let's query the database. We'll convert the values from the `SearchTerms` column and use a special `TsVector` method called `Matches` to seek records by the `Hello World` pattern:

```csharp
db.Records.Where(x =>
    EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("Hello World")
)
```

With our result printed to the console, our experiment code should look like this:

```csharp
using var db = await Db.Seeded();

var result = await db.Records
    .Where(x =>
        EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("Hello World")
    )
    .ToListAsync();

if (!result.Any())
    Console.WriteLine("No records found.");

foreach (var record in result)
    Console.WriteLine($"record: {record.Id} = '{record.SearchTerms}'");
```

This should give us the output below:

```text
record: 3ea00302-5dbd-468d-a41e-3b97155e3f28 = 'Hello World!'
```

Our search is working! But it's not that exciting since we match the words almost literally. Let's do something much more interesting in the next section.

## Using Advanced Text Search Patterns

In this section, we will conduct a few experiments with different search scenarios and see how to handle them properly.

First, let's check if our queries are **case insensitive**. Will a lower-case `hello` return any matching record?

```csharp
var result = await db.Records
    .Where(x =>
        EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("hello")
    )
    .ToListAsync();
```

This will in fact return the two matching records:

```text
record: 2c816ffc-712c-41e7-adae-add4734cb6eb = 'Hello World!'
record: b38fdc54-0601-43f2-b3d7-d2f5e8d83562 = 'Hello John!'
```

Now, let's say a user inserts the beginning of a name present in our search terms. We have a lot of Jacks in our database. Will they be returned if `ja` is our query string?

```csharp
var result = await db.Records
    .Where(x =>
        EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("ja")
    )
    .ToListAsync();
```

Here's what this is going to print:

```
No records found.
```

As you might see, the query didn't match any occurrence of certain letters. Instead, it seems to match **the whole word by default**. But what if we want a user to see suggestions before the exact match? Turns out, there's a special text search syntax in PostgreSQL. In our case, we'll need our query letters to match **just the beginning of the word**. We can achieve that with `:*` suffix:

```csharp
var result = await db.Records
    .Where(x =>
        x.SearchVector.Matches(
            EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("ja:*")
        )
    )
    .ToListAsync();
```

Now, we will see our Jacks:

```text
record: 0a723dda-a785-45ff-a51c-2759113273fc = 'Jack Black'
record: 6babd703-97ed-4373-835b-5b6ac733e7df = 'Jack Custome'
record: eb61ab0a-2773-4422-9c92-0856dc31756b = 'Jack Brown'
```

All of our trials so far have found words at the beginning of the text. `John`, on the other hand, is always placed at the end of the texts in our database. Let's see if we will be able to find text by a word that is **not at the start of the text**:

```csharp
var result = await db.Records
    .Where(x =>
        EFHelpers.Functions.ToTsVector(x.SearchTerms).Matches("john")
    )
    .ToListAsync();
```

And this last experiment will give us our Johns back.

```text
record: 4ef01914-15c8-4f26-89d9-a0142a027a28 = 'Bye, John!'
record: 585742e1-8dd9-4acd-9fbe-a828af5f728a = 'Hello John!'
```

We've seen enough of the text searches. We have one more vital point to cover. Let's get to it in the next section.

## Improving Performance With TsVector Columns

Our queries so far have been converting text in our column to a search vector on the fly. It was fine for our 7-records database. However, this will be problematic with any significant data set. Instead, we should have a column already containing our search vector:

```csharp
public NpgsqlTsVector SearchVector { get; set; } = null!;
```

In order to fill the column, we'll use automatic generation from the `SearchTerms` column. Additionally, we'll index the column with a Generic-Inverted Index (GIN) to get sensible query performance:

```csharp
modelBuilder.Entity<DbRecord>()
    .HasGeneratedTsVectorColumn(
        p => p.SearchVector,
        "english",
        p => p.SearchTerms
    )
    .HasIndex(p => p.SearchVector)
    .HasMethod("GIN");
```

Here's how our complete database code will look after the updates:

```csharp
public class DbRecord
{
    public required string Id { get; set; }
    public required string SearchTerms { get; set; }
    public NpgsqlTsVector SearchVector { get; set; } = null!;

    public static DbRecord New(string searchTerms) => new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = searchTerms
    };
}

public class Db : DbContext
{
    public DbSet<DbRecord> Records { get; set; }

    public static async Task<Db> Seeded()
    {
        var db = new Db();
        var seeds = Seeds.As(DbRecord.New);
        await db.EnsureRecreated(x => x.Records.AddRange(seeds));
        return db;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UsePostgres("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbRecord>()
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",
                p => p.SearchTerms
            )
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
    }
}
```

Now, we will be able to omit the conversion code and match straight against the `SearchVector` column.

Let's check it out with a fancy query for all Jacks, having their last name starting with the `b`:

```csharp
var result = await db.Records
    .Where(x =>
        x.SearchVector.Matches(
            EFHelpers.Functions.ToTsQuery("jack & b:*")
        )
    )
    .ToListAsync();
```

The code should give us a proper result:

```
record: 9d59f3da-7e9a-403d-bc19-6b6b72caefbe = 'Jack Black'
record: f04ea976-0e89-4353-b43f-39cb4de731d4 = 'Jack Brown'
```

This wraps up the main part of the article. However, there's a bonus in the last section, simplifying working with queries even more. Let's get closer to the finish line!

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
