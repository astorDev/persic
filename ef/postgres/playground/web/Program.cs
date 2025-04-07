using Microsoft.EntityFrameworkCore;
using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostgres<Db>();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<Db>();
var canConnect = await db.Database.CanConnectAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

app.Run();

public class Db(DbContextOptions<Db> options) : DbContext(options) {
}
