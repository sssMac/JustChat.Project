using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.DAL.Models.Settings
{
    public interface IMongoDBSettings
    {
        string CollectionName {get;set;}
        string ConnectionString { get; set; }
        string DatabaseName {get;set;}
        string User { get; set; }
        string Password { get; set; }
        string Host { get; set; }
        int Port { get; set; }
    }
}
