using Microsoft.EntityFrameworkCore;
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
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var setupTask = setup?.Invoke(context);
        if (setupTask != null) await setupTask;
    }
}