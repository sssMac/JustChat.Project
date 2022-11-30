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
    }
}
