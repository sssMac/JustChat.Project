using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.Models.ViewModel
{
    public class OnlineUser
    {
        public string ConnectionId { get; set; }
        public string Id { get; set; }
        public DateTime TimeJoined { get; set; }
    }
}
