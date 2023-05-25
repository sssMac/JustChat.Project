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
            (
                collectionName: config.GetSection("MongoDBSettings:CollectionName").Value,
                databaseName: config.GetSection("MongoDBSettings:DatabaseName").Value,
                user: config.GetSection("MongoDBSettings:User").Value,
                password: config.GetSection("MongoDBSettings:Password").Value,
                host : config.GetSection("MongoDBSettings:Host").Value,
                port: int.Parse(config.GetSection("MongoDBSettings:Port").Value)
            );

            services.AddSingleton<IMongoDBSettings>(s => new MongoDBSettings
            (
                collectionName: config.GetSection("MongoDBSettings:CollectionName").Value,
                databaseName: config.GetSection("MongoDBSettings:DatabaseName").Value,
                user: config.GetSection("MongoDBSettings:User").Value,
                password: config.GetSection("MongoDBSettings:Password").Value,
                host: config.GetSection("MongoDBSettings:Host").Value,
                port: int.Parse(config.GetSection("MongoDBSettings:Port").Value)
            ));
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
