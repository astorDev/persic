using Microsoft.EntityFrameworkCore;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public sealed class Ping
{
    [TestMethod]
    public async Task Connected()
    {
        using var db = new ThisDb();
        await db.Database.OpenConnectionAsync();
    }
}

public class ThisDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UsePostgres("Host=localhost;Port=5631;Database=postgres_search;Username=postgres_search;Password=postgres_search");
    }
}