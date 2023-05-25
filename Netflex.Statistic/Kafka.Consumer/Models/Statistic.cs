using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kafka.Consumer.Models
{
    [BsonIgnoreExtraElements]
    public class Statistic
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;

        [BsonElement("contentId")]
        public Guid ContentId { get; set; }

        [BsonElement("contentName")]
        public string ContentName { get; set; } = String.Empty;

        [BsonElement("viewCount")]
        public int ViewCount { get; set; }
    }
}
