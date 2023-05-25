
using Newtonsoft.Json;
namespace Netflex.Kafka.Consumer
{
    public class StatisticProcessingRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
