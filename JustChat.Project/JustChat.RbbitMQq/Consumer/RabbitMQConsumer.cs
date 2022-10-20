using BLL.Interfaces;
using DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustChat.RbbitMQ.Consumer
{
    public class RabbitMQConsumer : BackgroundService

    {
        private readonly    IServiceScopeFactory    _scopeFactory;
        private readonly    ILogger                 _logger;
        private readonly    IMessageService         _messageService;
        private             IConnection?            _connection;
        private             IModel?                 _channel;
        private             string?                 _queueName;

        public RabbitMQConsumer(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<RabbitMQConsumer>();
            _scopeFactory = scopeFactory;
            _messageService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMessageService>();

            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();
            

            _channel.ExchangeDeclare(exchange: "notifier", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                                      exchange: "notifier",
                                      routingKey: string.Empty);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                // received message  
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                var mesageData = JsonConvert.DeserializeObject<Message>(message);
                await _messageService.AddMessageAsync(mesageData);

                Console.WriteLine(ea.Body);
                // handle the received message  
                HandleMessage(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };


            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // we just print this message   
            _logger.LogInformation($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

