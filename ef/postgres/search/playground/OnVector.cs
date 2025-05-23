using Microsoft.EntityFrameworkCore;
using Shouldly;
using EFHelpers = Microsoft.EntityFrameworkCore.EF;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public class OnVector
{
    [TestMethod]
    public async Task HelloWorld()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .Where(x =>
                x.SearchVector.Matches("Hello World")
            )
            .ToListAsync();

        Assert(result, 1);
    }

    [TestMethod]
    public async Task Hello()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .Where(x =>
                x.SearchVector.Matches("hello")
            )
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Bye()
    {
        using var db = SeededDb.New();

        var result = await db.Records
            .Where(x =>
                x.SearchVector.Matches("bye")
            )
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task Jo()
    {
        using var db = SeededDb.New();
        await db.SaveChangesAsync();

        var result = await db.Records
            .Where(x =>
                x.SearchVector.Matches(
                    EFHelpers.Functions.ToTsQuery("jo:*")
                )
            )
            .ToListAsync();

        Assert(result, 2);
    }

    [TestMethod]
    public async Task JackB()
    {
        using var db = SeededDb.New();
        await db.SaveChangesAsync();

        var result = await db.Records
            .Where(x =>
                x.SearchVector.Matches(
                    EFHelpers.Functions.ToTsQuery("jack & b:*")
                )
            )
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