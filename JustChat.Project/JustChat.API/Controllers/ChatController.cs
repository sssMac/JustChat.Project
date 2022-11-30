using Amazon.S3.Model;
using BLL.Interfaces;
using DAL.Entities;
using JustChat.API.Cache;
using JustChat.API.Hubs;
using JustChat.BLL.Commands;
using JustChat.BLL.Interfaces;
using JustChat.BLL.Queries;
using JustChat.DAL.Entities;
using JustChat.DAL.ViewModel;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

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
        private IRabbitMQService _rabbitMQService;
        private readonly IDistributedCache _cache;

        public ChatController(
            IMessageService messageService,
            IFileService fileService,
            IMediator mediator,
            IHubContext<ChatHub> hubContext,
            IRabbitMQService rabitMQService,
            IDistributedCache cache)
        {
            _messageService = messageService;
            _fileService = fileService;
            _mediator = mediator;
            _hubContext = hubContext;
            _rabbitMQService = rabitMQService;
            _cache = cache;
        }

        [HttpGet("chathistory")]
        //[Cached(600)]
        public async Task<IActionResult> GetChatHistory()
        {


            string cacheKey = "cacheKey";
            byte[] cachedData = await _cache.GetAsync(cacheKey);
            List<MessageResponse> chatHistory = new();

            if (cachedData != null)
            {
                // If the data is found in the cache, encode and deserialize cached data.
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                chatHistory = JsonSerializer.Deserialize<List<MessageResponse>>(cachedDataString);
            }
            else{
                var messages = await _messageService.GetChatHistoryAsync();
                var files = await _fileService.GetAllFilesAsync();

                var res = from message in messages
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

                string cachedDataString = JsonSerializer.Serialize(res);
                var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(3));

                await _cache.SetAsync(cacheKey, dataToCache, options);
            }

            return Ok(chatHistory);
        }

        [HttpDelete("deletemessage")]
        public async Task<bool> DeleteMessage(Guid id)
        {
            return await _messageService.DeleteMessageAsync(id);
        }

        [HttpPost("postmessage")]
        public async Task<IActionResult> PostMessage([FromForm] MessageRequest mess)
        {
            mess.MessageId = Guid.NewGuid();
            mess.PublishDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            Message message = new Message
            {
                MessageId = mess.MessageId,
                UserName = mess.UserName,
                Text = mess.Text,
                PublishDate = mess.PublishDate
            };

            //_rabbitMQService.SendMessage(message);
            await _messageService.AddMessageAsync(message);

            var newFile = new MetaFile();

            if (mess.Image != null)
            {
                newFile = await _fileService.PostFileAsync(mess);

            }

            var response = new MessageResponse
            {
                message = message,
                rsp = mess.Image != null? newFile : null

            };

            await _hubContext.Clients.All.SendAsync("SendMessage", response);
            await _cache.RemoveAsync("cacheKey");
            return Ok("Message send!");
        }


        [HttpPost("createbucket")]
        public async Task<IActionResult> CreateBucket(string name )
        {
            await _fileService.CreateBucketAsync(name);
            return Ok("Bucket created!");
        }

        //[HttpPost("uploadfile")]
        //public async Task<IActionResult> PostFile( IFormFile file )
        //{
        //    var fileData = await _fileService.PostFileAsync(file);
        //
        //    await _hubContext.Clients.All.SendAsync("FileMessage", fileData);
        //
        //    return Ok("File upload!");
        //}

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

