using System.Text;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;

namespace Persic;

public static class BigQueryPrintExtensions
{
    public static string ToString(this BigQueryRow row, string separator = ", ")
    {
        var values = row.RawRow.F.Select(f => f.V);
        return string.Join(separator, values);
    }

    public static string ToString(this TableSchema schema, string separator = ", ", Func<TableFieldSchema, string>? selector = null)
    {
        selector ??= f => f.Name;
        var names = schema.Fields.Select(selector);
        return string.Join(separator, names);
    }

    public static string ToMarkdownTableHeader(this TableSchema schema)
    {
        var header = schema.ToString(" | ");
        var separator = schema.ToString(" | ", x => "---");
        return header + "\n" + separator;
    }

    public static string ToMarkdownTable(this BigQueryResults results)
    {
        var sb = new StringBuilder();
        sb.AppendLine(results.Schema.ToMarkdownTableHeader());
        foreach (var row in results)
        {
            sb.AppendLine(row.ToString(" | "));
        }

        return sb.ToString();
    }
}
