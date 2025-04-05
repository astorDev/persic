using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public static class MigrateExternsions
{
    public static async Task Migrate<T>(this IServiceProvider services) where T : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.Database.MigrateAsync();
    }

    public static async Task EnsureRecreated<T>(this IServiceProvider services) where T : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}