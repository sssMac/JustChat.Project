﻿using JustChat.API.Hubs;
using JustChat.BLL.Services;
using Microsoft.AspNet.SignalR;

namespace JustChat.API.StartUp
{
    public static class SignalRConfiguration
    {
        public static WebApplication ConfigureSignalR(this WebApplication app)
        {
            GlobalHost.DependencyResolver.Register(
                    typeof(ChatHub),
                    () => new ChatHub(new RabbitMQService()));

            app.MapHub<ChatHub>("/hub");
            return app;
        }
    }
}