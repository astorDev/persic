using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Persic.EF.Postgres.Search;

namespace Persic.EF.Postgres.Playground;

public class DbRecord : IRecordWithSearchVector
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
    public static readonly DbRecord HelloWorld = DbRecord.New("Hello World!");
    public static readonly DbRecord HelloJohn = DbRecord.New("Hello John!");
    public static readonly DbRecord ByeJohn = DbRecord.New("Bye, John!");
    public static readonly DbRecord Bye = DbRecord.New("Bye!");
    public static readonly DbRecord JackBlack = DbRecord.New("Jack Black");
    public static readonly DbRecord JackCustome = DbRecord.New("Jack Custome");
    public static readonly DbRecord JackBrown = DbRecord.New("Jack Brown");

    public static readonly DbRecord[] Records =
    [
        HelloWorld,
        HelloJohn,
        ByeJohn,
        Bye,
        JackBlack,
        JackCustome,
        JackBrown
    ];

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