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

            services.AddSingleton<IMongoClient>(s =>
                new MongoClient(config.GetValue<string>("MongoDBSettings:ConnectionString")));

            return services;
        }
    }
}
