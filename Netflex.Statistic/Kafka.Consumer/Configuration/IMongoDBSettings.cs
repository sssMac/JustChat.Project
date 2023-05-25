namespace Kafka.Consumer.Configuration
{
    public interface IMongoDBSettings
    {
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}