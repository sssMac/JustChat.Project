using BLL.Interfaces;
using BLL.Services;
using MediatR;
using DAL.Data;
using JustChat.BLL.Interfaces;
using JustChat.BLL.Services;
using JustChat.RbbitMQ.Consumer;

namespace JustChat.API.StartUp
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddCors();
            services.AddSignalR();
            services.AddAntiforgery(o => o.HeaderName = "X-XSRF-TOKEN");
            services.AddSingleton<IUsersManager, UsersManager>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IMessageService, MessageService>();
            //services.AddTransient<IRabbitMQService, RabbitMQService>();
            //services.AddHostedService<RabbitMQConsumer>();
            services.AwsConnect(config);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMediatR(typeof(MessageService).Assembly);


            return services;
        }
    }
}
