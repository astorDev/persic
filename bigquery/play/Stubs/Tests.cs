namespace Persic.BigQuery.Playground;

[TestClass]
public class Stubs
{
    [TestMethod] public void Single() => RunFile("single");
    [TestMethod] public void Multi() => RunFile("multi");

    public void RunFile(string name)
    {
        var sql = File.ReadAllText($"../../../Stubs/{name}.sql");
        var result = Context.Client.ExecuteQuery(sql, null);

        var markdownTable = result.ToMarkdownTable($"Query Result (top 50 of {result.TotalRows})", limit: 50);
        Console.WriteLine(markdownTable);
        File.WriteAllText($"../../../Stubs/buffer.{name}.md", markdownTable);
        File.WriteAllText($"../../../Stubs/buffer.{name}.csv", result.ToCsv(limit: 50));
    }
}