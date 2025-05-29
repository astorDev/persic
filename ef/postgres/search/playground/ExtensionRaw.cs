using Microsoft.EntityFrameworkCore;
using Shouldly;
using Persic.EF.Postgres.Search;
using NpgsqlTypes;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public class ExtensionRaw
{
    [TestMethod]
    public async Task HelloWorld()
    {
        using var db = await Db.Seeded();

        var result = await db.Records
            .WhereSearchVectorMatches("hello & world")
            .ToListAsync();

        Assert(result, 1);
    }

    [TestMethod]
    public async Task Hello()
    {
        using var db = await Db.Seeded();

        var result = await db.Records
            .WhereSearchVectorMatches("hello")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Bye()
    {
        using var db = await Db.Seeded();

        var result = await db.Records
            .WhereSearchVectorMatches("bye")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Jo()
    {
        using var db = await Db.Seeded();

        var result = await db.Records
            .WhereSearchVectorMatches("jo:*")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task JackB()
    {
        using var db = await Db.Seeded();

        var result = await db.Records
            .WhereSearchVectorMatches("jack & b:*")
            .ToListAsync();

        Assert(result, 2);
    }

    public void Assert(IEnumerable<DbRecord> result, int count)
    {
        result.Count().ShouldBe(count);

        foreach (var record in result)
        {
            Console.WriteLine($"record: {record.Id} = '{record.SearchTerms}'");
        }
    }

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
                .HasIndexedSearchVectorGeneratedFrom(
                    p => p.SearchTerms
                );
        }
    }
}