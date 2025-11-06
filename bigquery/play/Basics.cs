using System.Diagnostics;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

namespace Persic.BigQuery.Playground;

[TestClass]
public class Basics
{
    [TestMethod]
    public async Task Connect()
    {
        var datasets = await Context.Client.ListDatasetsAsync("calm-cascade-477407-g2").ReadPageAsync(10);
        foreach (var dataset in datasets) Console.WriteLine(dataset.FullyQualifiedId);

        datasets.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task Fill()
    {
        var ds = await Context.Client.GetOrCreateDatasetAsync("play_autotests_data");
        ds.ShouldNotBeNull();
        Console.WriteLine(ds.FullyQualifiedId);

        var tableSchema = new TableSchemaBuilder
        {
            { "name", BigQueryDbType.String },
            { "age", BigQueryDbType.Int64 }
        }.Build();

        var table = await ds.GetOrCreateTableAsync("users", tableSchema);
        table.ShouldNotBeNull();

        var results = await Context.Client.InsertRowsAsync(Context.Client.ProjectId, ds.Reference.DatasetId, table.Reference.TableId, [
            new BigQueryInsertRow { { "name", "Alice" }, { "age", 30 } },
            new BigQueryInsertRow { { "name", "Bob" }, { "age", 25 } },
        ]);

        results.Status.ShouldBe(BigQueryInsertStatus.AllRowsInserted);
    }

    [TestMethod]
    public void CountSync() => Count().GetAwaiter().GetResult();
    public async Task Count()
    {
        var sql = "SELECT COUNT(*) as total_users FROM `calm-cascade-477407-g2.play_autotests_data.users`";
        var result = await Context.Client.ExecuteQueryAsync(sql, parameters: null);
        var row = result.ToList().FirstOrDefault();
        row.ShouldNotBeNull();
        var totalUsers = (long)row["total_users"];
        Console.WriteLine($"Total users: {totalUsers}");
        totalUsers.ShouldBeGreaterThan(0);
    }
}