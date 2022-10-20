using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace JustChat.RabbitMQ.Producer
{
    public class RabbitMqService : IRabbitMqService
    {

        public bool SendMessage<T>(T message)
        {
            try
            {var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "notifier", type: ExchangeType.Fanout);

                        var json = JsonConvert.SerializeObject(message);
                        var messageBody = Encoding.UTF8.GetBytes(json);

                        channel.BasicPublish(exchange: "notifier",
                                            routingKey: "",
                                            basicProperties: null,
                                            body: messageBody);

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} | {ex.StackTrace}");
                return false;
            }


        }
    }
}
