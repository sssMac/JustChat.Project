using BLL.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BLL.Services
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
                        channel.QueueDeclare(queue: "JustChat",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                        var json = JsonConvert.SerializeObject(message);
                        var messageBody = Encoding.UTF8.GetBytes(json);

                        channel.BasicPublish(exchange: "", routingKey: "JustChat", body: messageBody, basicProperties: null);
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
