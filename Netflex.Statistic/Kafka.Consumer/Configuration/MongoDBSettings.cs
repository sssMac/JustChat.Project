namespace Kafka.Consumer.Configuration
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public MongoDBSettings(string collectionName,  string databaseName, string user, string password, string host, int port)
        {
            CollectionName = collectionName;
            DatabaseName = databaseName;
            User = user;
            Password = password;
            Host = host;
            Port = port;
        }

        public string CollectionName { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string User { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; }

    }
}
