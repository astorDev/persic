# Postgres Full Text Search with EF Core 9

> No Elasticsearch needed.

Text search is something relational databases have historically struggled with. This limitation even led to a rise of dedicated search tools, like Elasticsearch.

However, since Postgres 8.3, we got a thing called `tsvector`, providing us a powerful way to query our data straight from the relational database. In this article, we are going to play around with the text search in PostgreSQL, and build a few helpers using .NET 9 and Entity Framework Core.

> Or jump straight to the [TLDR](#tldr) in the end of this article for a short reference.

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

And finally query your record using a simple extension method:

```csharp
var result = await db.Records
            .WhereSearchVectorMatches("jack & b:*")
            .ToListAsync();
```

The package, as well as this article, is part of a project called [persic](https://github.com/astorDev/persic), containing various DB-related tooling. Check it out on [GitHub](https://github.com/astorDev/persic), and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉