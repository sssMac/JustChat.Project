using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.Models.ViewModel
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Group { get; set; }
    }
}
