using Newtonsoft.Json;

namespace Netflex.Statistic.Server.Models
{
    public class StatisticRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
