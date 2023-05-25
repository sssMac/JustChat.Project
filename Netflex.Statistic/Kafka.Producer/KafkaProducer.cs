using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Kafka.Producer 
{ 
    public class KafkaProducer : IHostedService
    {
        private string bootstrapServers = "";
        private readonly string topic = "topic";
        private IConfiguration _configuration;

        public KafkaProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            bootstrapServers = _configuration.GetSection("Kafka:URL").Value;
            
        }

        public async Task<bool> Post(StatisticRequest orderRequest)
        {

            string message = JsonSerializer.Serialize(orderRequest);
            return await SendStatisticRequest(topic, message);
        }

        private async Task<bool> SendStatisticRequest(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                AllowAutoCreateTopics = true,
            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    Console.WriteLine($" ----- > {message} FROM KafkaProducer");
                    var result = await producer.ProduceAsync
                    (topic, new Message<Null, string>
                    {
                        Value = message
                    });

                    Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");

                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult(false);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string message = JsonSerializer.Serialize(new StatisticRequest { Id = Guid.NewGuid(), Name = "TestName"});
            await SendStatisticRequest(topic, message);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
