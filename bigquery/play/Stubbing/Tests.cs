using Google.Cloud.BigQuery.V2;

namespace Persic.BigQuery.Playground;

[TestClass]
public class Stub
{
    [TestMethod] public void Single() => RunFile("single");
    [TestMethod] public void Multi() => RunFile("multi");

    public void RunFile(string name)
    {
        var sql = File.ReadAllText($"../../../Stubbing/{name}.sql");
        var result = Context.Client.ExecuteQuery(sql, null);
        result.PrintLineByLine(row => row.PrintView());
    }
}

public static class PrinterExtension
{
    public static void PrintLineByLine<T>(this IEnumerable<T> rows, Func<T, string> formatter)
    {
        foreach (var row in rows)
        {
            Console.WriteLine(formatter(row));
        }
    }

    public static string PrintView(this BigQueryRow row)
    {
        var values = row.RawRow.F.Select(f => f.V);
        return string.Join(", ", values);
    }
}