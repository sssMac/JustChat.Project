﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.BLL.Interfaces
{
    public interface IRabbitMQService
    {
        public bool SendMessage<T>(T message);
    }
}
