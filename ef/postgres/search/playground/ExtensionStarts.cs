using Microsoft.EntityFrameworkCore;
using Shouldly;
using Persic.EF.Postgres.Search;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public class ExtensionStarts
{
    [TestMethod]
    public async Task HelloWorld()
    {
        using var db = await SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorStartsWithAllIn("hello world")
            .ToListAsync();

        Assert(result, 1);
    }

    [TestMethod]
    public async Task Hello()
    {
        using var db = await SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorStartsWithAllIn("hello")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Bye()
    {
        using var db = await SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorStartsWithAllIn("bye")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Jo()
    {
        using var db = await SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorStartsWithAllIn("jo")
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task JackB()
    {
        using var db = await SeededDb.New();

        var result = await db.Records
            .WhereSearchVectorStartsWithAllIn("jack b")
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