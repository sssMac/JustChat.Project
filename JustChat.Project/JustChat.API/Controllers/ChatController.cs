using Amazon.S3.Model;
using BLL.Interfaces;
using DAL.Entities;
using JustChat.BLL.Hubs;
using JustChat.BLL.Commands;
using JustChat.BLL.Interfaces;
using JustChat.BLL.Queries;
using JustChat.DAL.Entities;
using JustChat.DAL.ViewModel;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using JustChat.API.Services;

namespace JustChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _hubContext;
        private ChatControll _chatControll;
        //private IRabbitMQService _rabbitMQService;
        private readonly IUsersManager _usersManager;


        public ChatController(
            IMessageService messageService,
            IFileService fileService,
            IMediator mediator,
            IHubContext<ChatHub> hubContext,
            //IRabbitMQService rabitMQService,
            IUsersManager usersManager,
            ChatControll chatControll)
        {
            _messageService = messageService;
            _fileService = fileService;
            _mediator = mediator;
            _hubContext = hubContext;
            //_rabbitMQService = rabitMQService;
            _usersManager = usersManager;
            _chatControll = chatControll;
        }

        [HttpGet("chathistory")]
        public async Task<IActionResult> GetChatHistory(string user)
        {

            List<MessageResponse> chatHistory = new();
            var currentUser = (await _usersManager.GetUsers()).FirstOrDefault(u => u.ConnectionId == user);
            var messages = await _messageService.GetChatHistoryAsync();
            var files = await _fileService.GetAllFilesAsync();

            var res = from message in messages
                      where message.UserName == currentUser.Id || message.Whom == currentUser.Id
                      join file in files
                      on message.MessageId equals file.MessageId
                      into MessageResponse
                      from rsp in MessageResponse.DefaultIfEmpty()
                      select new { message, rsp };

            foreach (var item in res)
            {
                chatHistory.Add(
                    new MessageResponse
                    {
                        message = item.message,
                        rsp = item.rsp,
                    }
                );
            }
            return Ok(chatHistory);

        }

        [HttpGet("onlineusers")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            var onlineUsers = await _usersManager.GetUsers();
            
            return Ok(onlineUsers);
        }

        [HttpDelete("deletemessage")]
        public async Task<bool> DeleteMessage(Guid id)
        {
            return await _messageService.DeleteMessageAsync(id);
        }

        [HttpPost("postmessage")]
        public async Task<IActionResult> PostMessage([FromForm] MessageRequest mess, string receiver)
        {
            mess.MessageId = Guid.NewGuid();
            mess.PublishDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            Message newMessage = new Message
            {
                MessageId = mess.MessageId,
                UserName = mess.UserName,
                Whom = mess.Whom,
                Text = mess.Text,
                PublishDate = mess.PublishDate
            };

            await _messageService.AddMessageAsync(newMessage);

            var newFile = new MetaFile();
            if (mess.Image != null)
            {
                newFile = await _fileService.PostFileAsync(mess);
            }


            var response = new MessageResponse
            {
                message = newMessage,
                rsp = mess.Image != null ? newFile : null

            };

            var user = await _chatControll.GetUserByNameAsync(mess.Whom);
            await _chatControll.SaveMessageToUsersAsync(user, mess.Text);

            await _hubContext.Clients.Client(receiver).SendAsync("ReceiveMessage", response);
            await _hubContext.Clients.Group(mess.Whom).SendAsync("ReceiveGroupMessage", response);

            return Ok("Message send!");
        }


        [HttpPost("createbucket")]
        public async Task<IActionResult> CreateBucket(string name )
        {
            await _fileService.CreateBucketAsync(name);
            return Ok("Bucket created!");
        }


        [HttpGet("files")]
        public async Task<List<MetaFile>> GetFiles( )
        {
            var files = await _fileService.GetAllFilesAsync();
            return files;
        }

        [HttpGet("buckets")]
        public async Task<List<S3Bucket>> GetBuckets()
        {
            var bucket = await _fileService.GetAllBucketsAsync();
            return bucket;
        }


        [HttpGet("sqrs/getMessages")]
        public async Task<List<Message>> Get()
        {
            return await _mediator.Send(new GetMessageListQuery());
        }

        [HttpGet("sqrs/getMessage/{id}")]
        public async Task<Message> Get(Guid id)
        {
            return await _mediator.Send(new GetMessageByIdQuery(id));
        }

        [HttpPost("sqrs/upostMessage")]
        public async Task<Message> Post(Message movieModel)
        {
            return await _mediator.Send(new AddMessageCommand(movieModel));
        }

    }
}

