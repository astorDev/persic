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