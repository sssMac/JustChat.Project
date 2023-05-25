using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kafka.Consumer.Configuration
{
    public static class MongoDbConfiguration
    {
        public static IServiceCollection RegisterMongoDB(this IServiceCollection services, IConfiguration config)
        {
            var settings = new MongoDBSettings
            {
                Host = config.GetSection("MongoDBSettings:Host").Value,
                Port = int.Parse(config.GetSection("MongoDBSettings:Port").Value),
                User = config.GetSection("MongoDBSettings:User").Value,
                Password = config.GetSection("MongoDBSettings:Password").Value,
                DatabaseName = config.GetSection("MongoDBSettings:DatabaseName").Value,
                CollectionName = config.GetSection("MongoDBSettings:CollectionName").Value,
            };

            services.AddSingleton<IMongoDBSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);
            services.AddSingleton<IMongoClient>(s =>
                new MongoClient(new MongoClientSettings()
                {
                    Server = new MongoServerAddress(settings.Host, settings.Port),
                    //Credential = MongoCredential.CreateCredential("admin", settings.User, settings.Password)
                }));
            return services;
        }
    }
}
