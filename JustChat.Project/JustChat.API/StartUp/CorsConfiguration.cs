namespace JustChat.API.StartUp
{
    public static class CorsConfiguration
    {
        public static IServiceCollection RegisterCors(this IServiceCollection services)
        {
            services.AddCors();

            return services;
        }
        public static WebApplication ConfigureCors(this WebApplication app)
        {
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            return app;
        }
    }
}
