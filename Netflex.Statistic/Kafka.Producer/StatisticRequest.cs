using Newtonsoft.Json;

namespace Kafka.Producer
{
    public class StatisticRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
