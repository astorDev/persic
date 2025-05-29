using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public static class MigrateExternsions
{
    public static async Task Migrate<T>(this IServiceProvider services, Func<T, Task>? setup = null) where T : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.Database.MigrateAsync();

        var setupTask = setup?.Invoke(context);
        if (setupTask != null) await setupTask;
    }

    public static async Task EnsureRecreated<T>(this IServiceProvider services, Func<T, Task>? setup = null) where T : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.EnsureRecreated(setup);
    }

    public static async Task EnsureRecreated<T>(this IServiceProvider services, Action<T>? setup = null) where T : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.EnsureRecreated(setup);
    }

    public static async Task EnsureRecreated<TContext>(this TContext context, Action<TContext>? setup = null)
        where TContext : DbContext
    {
        await context.Database.EnsureRecreated();

        setup?.Invoke(context);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureRecreated<TContext>(this TContext context, Func<TContext, Task>? setup = null)
        where TContext : DbContext
    {
        await context.Database.EnsureRecreated();

        var setupTask = setup?.Invoke(context);
        if (setupTask != null) await setupTask;

        await context.SaveChangesAsync();
    }
    
    public static async Task EnsureRecreated(this DatabaseFacade database)
    {
        await database.EnsureDeletedAsync();
        await database.EnsureCreatedAsync();
    }
}