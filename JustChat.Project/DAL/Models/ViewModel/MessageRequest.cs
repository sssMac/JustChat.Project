using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.ViewModel
{
    public class MessageRequest
    {
        public Guid MessageId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public long PublishDate { get; set; }
        public IFormFile? Image { get; set; }
    }
}
