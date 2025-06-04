namespace Persic;

public class ConnectionString(Dictionary<string, string> source) : Dictionary<string, string>(source)
{
    public static ConnectionString Parse(string rawConnectionString, string separator = ";", string keyValueSeparator = "=")
    {
        var parts = rawConnectionString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var dictionary = new Dictionary<string, string>();

        foreach (var part in parts)
        {
            var keyValue = part.Split(keyValueSeparator);
            if (keyValue.Length == 2)
            {
                dictionary[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        return new ConnectionString(dictionary);
    }

    public string GetRequiredStringValue(string key)
    {
        if (TryGetValue(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Key '{key}' not found in connection string.");
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        if (TryGetValue(key, out var value) && bool.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }
}
