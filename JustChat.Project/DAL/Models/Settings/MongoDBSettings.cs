﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.Models.Settings
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string CollectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string User { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }

    }
}
