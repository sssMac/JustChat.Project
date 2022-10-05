using DAL.Entities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace JustChat.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public ChatHub(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
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

            await _publishEndpoint.Publish<Message>(message);

            await Clients.All.SendAsync("ReceiveMessage", message);

        }
    }
}
