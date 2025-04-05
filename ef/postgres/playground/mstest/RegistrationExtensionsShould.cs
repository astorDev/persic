using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Persic.EF.Postgres.Playground;

[TestClass]
public sealed class RegistrationExtensionsShould
{
    [TestMethod]
    public async Task RegisterAndExecuteEnsureCreated()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = "Host=localhost;Port=5432;Database=exampledb;Username=postgres;Password=postgres"
            })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddPostgres<ExampleDb>()
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ExampleDb>();
        await db.Database.EnsureCreatedAsync();
    }
}

public class ExampleDb(DbContextOptions<ExampleDb> options) : DbContext(options)
{
    public DbSet<ExampleRecord> ExampleRecords { get; set; } = null!;
}

public class ExampleRecord
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}