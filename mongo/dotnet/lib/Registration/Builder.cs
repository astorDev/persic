using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Persic;

public partial record MongoRegistrationBuilder(IServiceCollection Services)
{
    public MongoRegistrationBuilder AddCollection<T>(string name)
    {
        Services.AddMongoCollection<T>(name);
        return this;
    }

    public MongoRegistrationBuilder AddOpenCollections()
    {
        Services.AddOpenMongoCollections();
        return this;
    }
}