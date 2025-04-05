using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persic.EF.Playground;

public static class Given
{
    public static TContext Empty<TContext>() where TContext : DbContext, new()
    {
        var context = new TContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
}

public class PlaygroundContext : DbContext
{
    public DbSet<PlaygroundEntity> Entities { get; set; } = null!;
    public DbSet<UninterfacedEntity> UninterfacedEntities { get; set; } = null!;
    public DbSet<IntIdEntity> IntIdEntities { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=playground;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention()
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }
}

public class PlaygroundEntity : IDbEntity<string>
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Status { get; set; }
}

public class IntIdEntity : IDbEntity<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Status { get; set; }
}

public class UninterfacedEntity
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Status { get; set; }
}