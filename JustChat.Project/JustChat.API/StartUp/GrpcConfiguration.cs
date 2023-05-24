using Amazon.Auth.AccessControlPolicy;
using JustChat.API.Services;

namespace JustChat.API.StartUp
{
    public static class GrpcConfiguration
    {
        public static IServiceCollection RegisterGrpc(this IServiceCollection services)
        {
            services.AddGrpc();
            
            return services;
        }

        public static WebApplication ConfigureGrpc(this WebApplication app)
        {


           

            return app;
        }
    }
}
