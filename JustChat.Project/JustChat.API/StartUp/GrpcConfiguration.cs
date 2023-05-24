﻿using JustChat.API.Services;

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
            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ChatService>().EnableGrpcWeb().RequireCors("grpc-cors-policy"); ;
            });
            return app;
        }
    }
}
