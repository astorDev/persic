﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Persic.EF.Playground;

[TestClass]
public sealed class PutShould
{
    [TestMethod]
    public async Task AddNewEntity()
    {
        var context = Given.Empty<PlaygroundContext>();

        var entity = new PlaygroundEntity { 
            Id = "fe0a74ee-4018-4b25-9618-17d0bfdc7c17",
            Name = "State One" 
        };

        await context.Entities.Put(entity);
        context.SaveChanges();

        context = new PlaygroundContext();
        var afterSaveEntity = context.Entities.Find(entity.Id);

        afterSaveEntity.ShouldNotBeNull();
        afterSaveEntity.Name.ShouldBe("State One");
    }

    [TestMethod]
    public async Task UpdateExistingEntity()
    {
        var context = Given.Empty<PlaygroundContext>();

        var entity = new PlaygroundEntity { 
            Id = "488dc522-670b-477f-849c-71d4f412a5e7",
            Name = "State One",
            Status = "Pending"
        };

        await context.Entities.Put(entity);
        context.SaveChanges();

        context = new PlaygroundContext();
        var updatedEntity = new PlaygroundEntity { 
            Id = "488dc522-670b-477f-849c-71d4f412a5e7",
            Name = "State Two",
            Status = "Processed"
        };

        await context.Entities.Put(updatedEntity);
        context.SaveChanges();

        context = new PlaygroundContext();
        var storedEntity = context.Entities.Find(entity.Id);

        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe("State Two");
        storedEntity.Status.ShouldBe("Processed");
    }

    [TestMethod]
    public async Task WorkWithoutInterface()
    {
        var context = Given.Empty<PlaygroundContext>();

        var entity = new UninterfacedEntity { 
            Id = "86f4b4dc-3e26-4367-904f-7773291739c0",
            Name = "State One",
            Status = "Pending"
        };

        await context.UninterfacedEntities.Put(entity, e => e.Id == entity.Id);
        context.SaveChanges();

        context = new PlaygroundContext();
        var updatedEntity = new UninterfacedEntity { 
            Id = "86f4b4dc-3e26-4367-904f-7773291739c0",
            Name = "State Two",
            Status = "Processed"
        };

        await context.UninterfacedEntities.Put(updatedEntity, e => e.Id == updatedEntity.Id);
        context.SaveChanges();

        context = new PlaygroundContext();
        var storedEntity = context.UninterfacedEntities.Find(entity.Id);

        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe("State Two");
        storedEntity.Status.ShouldBe("Processed");
    }
}

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

    public static PlaygroundContext Fresh()
    {
        var context = new PlaygroundContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

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

public class UninterfacedEntity
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Status { get; set; }
}