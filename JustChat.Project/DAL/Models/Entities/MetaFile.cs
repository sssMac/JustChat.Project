using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JustChat.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class MetaFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;

        [BsonElement("messageId")]
        public Guid MessageId { get; set; }

        [BsonElement("titile")]
        public string Titile { get; set; } = String.Empty;

        [BsonElement("userName")]
        public string UserName { get; set; } = String.Empty;

        [BsonElement("publishDate")]
        public long PublishDate { get; set; }

    }
}
