using System.Text;

namespace Persic;

public static class StreamExtensions
{
    public static string ReadAsString(this Stream stream, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        var result = reader.ReadToEnd();
        return result;
    }
}