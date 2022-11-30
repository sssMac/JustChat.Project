

namespace DAL.Entities
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public long PublishDate { get; set; }
    }
}
