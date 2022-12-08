using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
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
        public string Whom { get; set; }

        [JsonIgnore]
        public IFormFile? Image { get; set; }
    }

    public class MessageRequestMQ
    {
        public Guid MessageId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public long PublishDate { get; set; }
        public FormFileChild Image { get; set; }
    }

    public class FormFileChild : FormFile
    {
        public FormFileChild(HeaderDictionary headers, Stream baseStream, long baseStreamOffset, long length, string name, string fileName) : base(baseStream, baseStreamOffset, length, name, fileName)
        {
            Headers = headers;
        }
    }
}
