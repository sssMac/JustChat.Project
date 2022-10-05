using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    internal interface IRabbitMqService
    {
        public bool SendMessage<T>(T message);
    }
}
