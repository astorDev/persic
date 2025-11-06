using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

namespace Persic.BigQuery.Playground;

public class Context
{
    static GoogleCredential creds = GoogleCredential.FromFile("buffer.key.json");
    public readonly static BigQueryClient Client = BigQueryClient.Create("calm-cascade-477407-g2", creds);
}