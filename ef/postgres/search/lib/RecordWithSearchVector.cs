using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using EFHelpers = Microsoft.EntityFrameworkCore.EF;

namespace Persic.EF.Postgres.Search;

public interface IRecordWithSearchVector
{
    NpgsqlTsVector SearchVector { get; }
}

public static class RecordWithSearchVectorExtensions
{
    public static EntityTypeBuilder<TEntity> HasIndexedSearchVectorGeneratedFrom<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, object>> includeExpression,
        string configuration = "english"
    )
        where TEntity : class, IRecordWithSearchVector
    {
        builder.HasGeneratedTsVectorColumn(
                tsVectorPropertyExpression: x => x.SearchVector,
                config: configuration,
                includeExpression: includeExpression
            )
            .HasIndex(x => x.SearchVector)
            .HasMethod("GIN");

        return builder;
    }

    public static IQueryable<TEntity> WhereSearchVectorStartsWithAllIn<TEntity>(
        this IQueryable<TEntity> source,
        Words words,
        string configuration = "english"
    )
        where TEntity : class, IRecordWithSearchVector
    {
        var query = PostgresTsQueryString.StartsWithAllIn(words);

        return source.Where(x => x.SearchVector.Matches(
            EFHelpers.Functions.ToTsQuery(
                configuration,
                query
            )
        ));
    }

    public static IQueryable<TEntity> WhereSearchVectorMatches<TEntity>(
        this IQueryable<TEntity> source,
        string tsQueryString,
        string configuration = "english"
    )
        where TEntity : class, IRecordWithSearchVector
    {
        return source.Where(x => x.SearchVector.Matches(
            EFHelpers.Functions.ToTsQuery(
                configuration,
                tsQueryString
            )
        ));
    }
}

public static class PostgresTsQueryString
{
    public static string StartsWithAllIn(
        Words words
    )
    {
        return string.Join(" & ", words.Select(x => $"{x}:*"));
    }
}

public record Words(string[] Raw) : IEnumerable<string>
{
    public static Words From(string rawString)
    {
        var parts = rawString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new Words(parts);
    }

    public static implicit operator string(Words words)
    {
        return string.Join(" ", words.Raw);
    }

    public static implicit operator Words(string rawString)
    {
        return From(rawString);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>)Raw).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}