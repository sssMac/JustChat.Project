using Netflex.Statistic.Server.Kafka;
using Netflex.Statistic.Server.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Netflex.Statistic.Server.RabitMQProducer
{
    public class RabitMQProducer : IRabitMQProducer
    {
        ConnectionFactory _factory;
        IKafkaProducer _kafkaProducer;

        public RabitMQProducer(IKafkaProducer kafkaProducer)
        {
            _factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "192.168.3.49"
            };
            _kafkaProducer = kafkaProducer;
        }

        public async Task SendStatistic(string request)
        {
            var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("Statistic", exclusive: false);

            var statistic = JsonConvert.DeserializeObject<StatisticRequest>(request);

            var body = Encoding.UTF8.GetBytes(statistic.Name);
            await _kafkaProducer.Post(statistic);

            channel.BasicPublish(exchange: "", routingKey: "logout", body: body);
            Console.WriteLine($"{statistic.Name} has been visited!");
        }
    }
}
