using Microsoft.Extensions.DependencyInjection;

namespace Persic;

public partial record MongoRegistrationBuilder(IServiceCollection Services)
{
    public MongoRegistrationBuilder AddCollection<T>(string name)
    {
        Services.AddMongoCollection<T>(name);
        return this;
    }
}