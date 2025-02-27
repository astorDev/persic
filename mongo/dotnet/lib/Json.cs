using System.Text.Json;
using MongoDB.Bson;

namespace Persic;

public static class Json
{
    public static async Task<JsonDocument> ToJsonDocument(this Task<BsonDocument> bsonTask) => 
        (await bsonTask).ToJsonDocument();

    public static JsonDocument ToJsonDocument(this BsonDocument bson) => 
        JsonDocument.Parse(bson.ToJson());

    public static JsonElement ToJsonElement(this BsonDocument bson) => 
        bson.ToJsonDocument().RootElement;

    public static BsonDocument ToBsonDocument(this JsonElement json) =>
        BsonDocument.Parse(json.GetRawText());
}