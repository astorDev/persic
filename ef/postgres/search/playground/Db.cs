using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Persic.EF.Postgres.Search;

namespace Persic.EF.Postgres.Playground;

public class DbRecord : IRecordWithSearchVector
{
    public required string Id { get; set; }
    public required string SearchTerms { get; set; }
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}

public class Db : DbContext
{
    public DbSet<DbRecord> Records { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search")
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<DbRecord>()
        //     .HasGeneratedTsVectorColumn(
        //         p => p.SearchVector,
        //         "english",
        //         p => p.SearchTerms
        //     )
        //     .HasIndex(p => p.SearchVector)
        //     .HasMethod("GIN");

        modelBuilder.Entity<DbRecord>()
            .HasIndexedSearchVectorGeneratedFrom(
                p => p.SearchTerms
            );
    }
}

public static class SeededDb
{
    public static DbRecord RecordHelloWorld = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Hello World!"
    };

    public static DbRecord RecordHelloJohn = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Hello John!"
    };

    public static DbRecord RecordByeJohn = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Bye, John!"
    };

    public static DbRecord RecordBye = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Bye!"
    };

    public static DbRecord RecordJackBlack = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Jack Black"
    };

    public static DbRecord RecordJackCustome = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Jack Custome"
    };

    public static DbRecord RecordJackBrown = new()
    {
        Id = Guid.NewGuid().ToString(),
        SearchTerms = "Jack Brown"
    };

    public static DbRecord[] Records = new[]
    {
        RecordHelloWorld,
        RecordHelloJohn,
        RecordByeJohn,
        RecordBye,
        RecordJackBlack,
        RecordJackCustome,
        RecordJackBrown
    };

    public static Db New()
    {
        var db = new Db();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.Records.AddRange(Records);
        db.SaveChanges();

        return db;
    }
}