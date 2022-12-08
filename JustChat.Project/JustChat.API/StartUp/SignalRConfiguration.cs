using JustChat.BLL.Hubs;

namespace JustChat.API.StartUp
{
    public static class SignalRConfiguration
    {
        public static IServiceCollection RegisterSignalR(this IServiceCollection services)
        {
            services.AddCors();

            return services;
        }
        public static WebApplication ConfigureSignalR(this WebApplication app)
        {

            app.MapHub<ChatHub>("/hub");
            return app;
        }
    }
}
