using Confluent.Kafka;
using Kafka.Consumer.Configuration;
using Kafka.Consumer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Kafka.Consumer.Consumer
{
    public class KafkaConsumer : IHostedService
    {
        private readonly string topic = "topic";
        private readonly string groupId = "statistics_group";
        private readonly string bootstrapServers = "";
        private IMongoCollection<Statistic> _statistics;
        private IConfiguration _configuration;

        public KafkaConsumer(IMongoDBSettings mongoDBSettings, IMongoClient mongoClient, IConfiguration configuration)
        {
            var mongoDB = mongoClient.GetDatabase("JustChat");
            _statistics = mongoDB.GetCollection<Statistic>("Statistics");
            _configuration = configuration;
            bootstrapServers = _configuration.GetSection("Kafka:URL").Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"TEST Kafka url :  {_configuration.GetSection("Kafka:URL").Value}");
            Console.WriteLine($"KafkaConsumer START!");

            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true,
            };


            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    var response = consumer.Consume(cts.Token);
                    if(response.Message != null)
                    {
                        var result = JsonConvert.DeserializeObject<StatisticProcessingRequest>(response.Message.Value);
                        Console.WriteLine($"Consumed message '{response.Message}' at '{response.TopicPartitionOffset}'");

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
