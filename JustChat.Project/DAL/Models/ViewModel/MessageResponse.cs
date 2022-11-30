using DAL.Entities;
using JustChat.DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.ViewModel
{
    public class MessageResponse
    {
        public Message message { get; set; }
        public MetaFile rsp { get; set; }
    }
}
