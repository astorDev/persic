namespace Persic.Files.Playground;

[TestClass]
public class HostReplacement
{
    [TestMethod]
    public void Domain()
    {
        var source = "http://localhost:9000/tests/presigned-uploaded.txt?X-Amz-Expires=600&X-Amz-Algorithm=AWS4-HMAC-SHA256";

        var result = source.ToUri().WithBaseUrl("https://tests.com");

        result.ToString().ShouldBe("https://tests.com/tests/presigned-uploaded.txt?X-Amz-Expires=600&X-Amz-Algorithm=AWS4-HMAC-SHA256");
    }

    [TestMethod]
    public void IpAndPort()
    {
        var source = "http://localhost:9000/tests/presigned-uploaded.txt?X-Amz-Expires=600&X-Amz-Algorithm=AWS4-HMAC-SHA256";

        var result = source.ToUri().WithBaseUrl("http://1.2.3.4:3214");

        result.ToString().ShouldBe("http://1.2.3.4:3214/tests/presigned-uploaded.txt?X-Amz-Expires=600&X-Amz-Algorithm=AWS4-HMAC-SHA256");
    }
}