using Amazon.S3.Model;
using BLL.Interfaces;
using DAL.Entities;
using JustChat.BLL.Commands;
using JustChat.BLL.Interfaces;
using JustChat.BLL.Queries;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private readonly IMediator _mediator;

        public ChatController(IMessageService messageService, IFileService fileService, IMediator mediator)
        {
            _messageService = messageService;
            _fileService = fileService;
            _mediator = mediator;
        }

        [HttpGet("chathistory")]
        public async Task<IEnumerable<Message>> GetChatHistory()
        {
            return await _messageService.GetChatHistoryAsync();
        }

        [HttpDelete("deletemessage")]
        public async Task<bool> DeleteMessage(Guid id)
        {
            return await _messageService.DeleteMessageAsync(id);
        }



        [HttpPost("createbucket")]
        public async Task<IActionResult> CreateBucket(string name )
        {
            await _fileService.CreateBucketAsync(name);
            return Ok("Bucket created!");
        }

        [HttpPost("uploadfile")]
        public async Task<IActionResult> PostFile( IFormFile file )
        {
            var fileData = await _fileService.PostFileAsync(file);

            return Ok("File upload!");
        }

        [HttpGet("files")]
        public async Task<List<S3Object>> GetFiles( string bucketName)
        {
            var file = await _fileService.GetAllFilesAsync(bucketName);
            return file;
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

