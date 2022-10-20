namespace JustChat.API.StartUp
{
    public static class CorsConfiguration
    {
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
