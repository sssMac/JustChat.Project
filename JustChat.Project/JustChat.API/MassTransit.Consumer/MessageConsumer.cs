using BLL.Interfaces;
using DAL.Entities;
using MassTransit;
using Newtonsoft.Json;

namespace JustChat.API.MassTransit.Consumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly IMessageService _messageService;

        public MessageConsumer(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Consumer message: {context}");

            var mesageData = new Message
            {
                MessageId = context.Message.MessageId,
                UserName = context.Message.UserName,
                Text = context.Message.Text,
                PublishDate = context.Message.PublishDate
            };

            await _messageService.AddMessageAsync(mesageData);
        }

    }
}

