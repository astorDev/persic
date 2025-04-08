using Confi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Persic;

public static class PostgresRegistrationExtensions
{
    public static IServiceCollection AddPostgres<TDbContext>(this IServiceCollection services, string configurationPath = "ConnectionStrings:Postgres", Action<NpgsqlDbContextOptionsBuilder>? configure = null)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetRequiredValue(configurationPath);
            options.UseNpgsql(connectionString, configure).UseSnakeCaseNamingConvention();
        });

        return services;
    }
}