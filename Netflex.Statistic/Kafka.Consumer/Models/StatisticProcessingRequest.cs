using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Consumer.Models
{
    public class StatisticProcessingRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
