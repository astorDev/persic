using Confi;
using Microsoft.EntityFrameworkCore;
using Persic;

await VFinal.Run(args);

public class V0
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddDbContext<Db>((sp, options) =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=playground");
        });

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
    }
}

public class V1
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddDbContext<Db>((sp, options) =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=playground");
        });

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Add(new Record { Name = "Test" });

        await db.SaveChangesAsync();

        var records = await db.Records.ToListAsync();
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
        public DbSet<Record> Records { get; set; } = null!;
    }

    public class Record
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

public class V2
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddDbContext<Db>((sp, options) =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=playground")
                .UseSnakeCaseNamingConvention();
        });

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Add(new Record { Name = "Test" });

        await db.SaveChangesAsync();

        var records = await db.Records.ToListAsync();
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
        public DbSet<Record> Records { get; set; } = null!;
    }

    public class Record
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

public class V3
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddDbContext<Db>((sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Postgres");

            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Add(new Record { Name = "Test" });

        await db.SaveChangesAsync();

        var records = await db.Records.ToListAsync();
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
        public DbSet<Record> Records { get; set; } = null!;
    }

    public class Record
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

public class V4
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddPostgreDbContext<Db>();

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Add(new Record { Name = "Test" });

        await db.SaveChangesAsync();

        var records = await db.Records.ToListAsync();
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
        public DbSet<Record> Records { get; set; } = null!;
    }

    public class Record
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreDbContext<TContext>(this IServiceCollection services, string configurationPath = "ConnectionStrings:Postgres")
        where TContext : DbContext
    {
        return services.AddDbContext<TContext>((sp, options) => {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetRequiredValue(configurationPath);

            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });
    }
}

public class VFinal
{
    public static async Task Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

        builder.Services.AddPostgres<Db>();

        var app = builder.Build();

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var canConnect = await db.Database.CanConnectAsync();
        app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        db.Add(new Record { Name = "Test" });

        await db.SaveChangesAsync();

        var records = await db.Records.ToListAsync();
    }

    public class Db(DbContextOptions<Db> options) : DbContext(options) {
        public DbSet<Record> Records { get; set; } = null!;
    }

    public class Record
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
