
using Newtonsoft.Json;
namespace Kafka.Consumer.Models
{
    public class StatisticProcessingRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
