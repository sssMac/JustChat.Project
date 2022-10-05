using BLL.Interfaces;
using DAL.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        //private readonly IRabbitMqService _rabitMqService;
        private readonly IPublishEndpoint _publishEndpoint;
        public ChatController(IMessageService messageService, IPublishEndpoint publishEndpoint)
        {
            _messageService = messageService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("chathistory")]
        public IEnumerable<Message> GetChatHistory()
        {
            var mesageList = _messageService.GetChatHistory();
            return mesageList;
        }

        [HttpGet("messagehistory")]
        public IEnumerable<Message> GetMessageHistory(string userName)
        {
            var userMesageList = _messageService.GetMessageHistory(userName);
            return userMesageList;
        }

        [HttpGet("getmessagebyid")]
        public Message GetMessageById(Guid id)
        {
            return _messageService.GetMessageById(id);
        }

        //[HttpPost("addmessage")]
        //public async Task<Message> AddMessage(Message message)
        //{
        //    var mesageData = _messageService.AddMessage(new Message
        //    {
        //        MessageId = Guid.NewGuid(),
        //        UserName = message.UserName,
        //        Text = message.Text,
        //        PublishDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
        //    });

        //    await _publishEndpoint.Publish(message);
        //    //_rabitMqService.SendMessage(mesageData);
        //    return message;

        //}

        [IgnoreAntiforgeryToken]
        [HttpPut("updatemessage")]
        public Message UpdateMessage(Message message)
        {
            return _messageService.UpdateMessage(message);
        }

        [HttpDelete("deletemessage")]
        public bool DeleteMessage(Guid id)
        {
            return _messageService.DeleteMessage(id);
        }
    }
}

