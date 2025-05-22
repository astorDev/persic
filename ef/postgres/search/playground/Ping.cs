using Microsoft.EntityFrameworkCore;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public sealed class Ping
{
    [TestMethod]
    public async Task Connected()
    {
        using var db = new Db();
        await db.Database.OpenConnectionAsync();
    }
}

public class Db : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search")
            .UseSnakeCaseNamingConvention();
    }
}