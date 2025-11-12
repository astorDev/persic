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

        var markdownTable = result.ToMarkdownTable();
        Console.WriteLine(markdownTable);
        File.WriteAllText($"../../../Stubs/buffer.{name}.md", markdownTable);
    }
}