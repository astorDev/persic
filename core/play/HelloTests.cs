using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Persic.Core.Playground;

[TestClass]
public class Confi
{
    [TestMethod]
    public void Parsed()
    {
        var raw = "Host=localhost;Port=8080;App=Thor";

        var connectionString = ConnectionString.Parse(raw);

        var config = connectionString.To<ConfiConfiguration>();

        config.Host.ShouldBe("localhost");
        config.Port.ShouldBe(8080);
        config.App.ShouldBe("Thor");
    }

    [TestMethod]
    public void ValidationError()
    {
        var raw = "Host=localhost;Port=9999999";

        var connectionString = ConnectionString.Parse(raw);

        try
        {
            connectionString.To<ConfiConfiguration>();
            throw new Exception("Expected OptionsValidationException was not thrown.");
        }
        catch (OptionsValidationException ex)
        {
            Console.WriteLine(ex);
        }
    }

    [TestMethod]
    public void AsConnectionString()
    {
        var uri = new Uri("http://localhost:8080/thor?refreshInterval=00:00:03");

        uri.Host.ShouldBe("localhost");
        uri.Port.ShouldBe(8080);
        uri.Scheme.ShouldBe("http");
        uri.LocalPath.ShouldBe("/thor");
        uri.Query.ShouldBe("?refreshInterval=00:00:03");
    }
}

public class ConfiConfiguration
{
    [Required]
    public string Host { get; init; } = null!;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required]
    public string App { get; init; } = null!;
}