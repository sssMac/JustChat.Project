using JustChat.BLL.Interfaces;
using JustChat.BLL.Services;
using JustChat.DAL.Models.Settings;

namespace JustChat.API.StartUp
{
    public static class CacheConfiguration
    {
        public static IServiceCollection RegisterRedisCache(this IServiceCollection services, IConfiguration config)
        {
            var redisCacheSettings = new RedisCacheSettings();
            config.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if (!redisCacheSettings.Enabled)
            {
                return services;
            }

            services.AddStackExchangeRedisCache(o => o.Configuration = redisCacheSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            return services;
        }
    }
}
