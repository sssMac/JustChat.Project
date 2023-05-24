namespace JustChat.API.StartUp
{
    public static class CorsConfiguration
    {
        public static IServiceCollection RegisterCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("grpc-cors-policy", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding"); ;
            }));

            return services;
        }
        public static WebApplication ConfigureCors(this WebApplication app)
        {
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());

            return app;
        }
    }
}
