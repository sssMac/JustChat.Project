using Amazon.S3.Model;
using BLL.Interfaces;
using DAL.Entities;
using JustChat.BLL.Interfaces;
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
        private readonly IFileService _fileService;
        public ChatController(IMessageService messageService, IFileService fileService)
        {
            _messageService = messageService;
            _fileService = fileService;
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
        public async Task<IActionResult> PostFile(string bucketName, IFormFile file )
        {
            var fileData = await _fileService.PostFileAsync(bucketName, file);

            return Ok("File upload!");
        }

        [HttpGet("files")]
        public async Task<List<S3Object>> GetFiles( string bucketName)
        {
            var file = await _fileService.GetAllFilesAsync(bucketName);
            return file;
        }


    }
}

