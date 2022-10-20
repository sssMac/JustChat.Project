using DAL.Entities;
using JustChat.BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace JustChat.API.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IRabbitMQService _rabitMQService;
        public ChatHub(IRabbitMQService rabitMQService)
        {
            _rabitMQService = rabitMQService;
        }

        public async Task SendMessage(string userName, string inputText)
        {
            Message message = new Message
            {
                MessageId = Guid.NewGuid(),
                UserName = userName,
                Text = inputText,
                PublishDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };

            _rabitMQService.SendMessage(message);

            await Clients.All.SendAsync("ReceiveMessage", message);

        }
    }
}
