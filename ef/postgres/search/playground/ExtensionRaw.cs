using Microsoft.EntityFrameworkCore;
using Shouldly;
using Persic.EF.Postgres.Search;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public class ExtensionRaw
{
    [TestMethod]
    public async Task HelloWorld()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorMatches("hello & world")
            .ToListAsync();

        Assert(result, 1);
    }

    [TestMethod]
    public async Task Hello()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorMatches("hello")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Bye()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorMatches("bye")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Jo()
    {
        using var db = SeededDb.New();
        await db.SaveChangesAsync();

        var result = await db.Records
            .WhereSearchVectorMatches("jo:*")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task JackB()
    {
        using var db = SeededDb.New();
        await db.SaveChangesAsync();

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
}