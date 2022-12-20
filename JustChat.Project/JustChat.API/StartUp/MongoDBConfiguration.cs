using JustChat.DAL.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JustChat.API.StartUp
{
    public static class MongoDBConfiguration
    {
        public static IServiceCollection RegisterMongoDB(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MongoDBSettings>(
                config.GetSection(nameof(MongoDBSettings)));

            services.AddSingleton<IMongoDBSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

            var settings =  new MongoDBSettings
            {
                Host = config.GetValue<string>("MongoDBSettings:Host"),
                Port = config.GetValue<int>("MongoDBSettings:Port"),
                User = config.GetValue<string>("MongoDBSettings:User"),
                Password = config.GetValue<string>("MongoDBSettings:Password"),
                DatabaseName = config.GetValue<string>("MongoDBSettings:DatabaseName"),
                CollectionName = config.GetValue<string>("MongoDBSettings:CollectionName"),
            };

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
