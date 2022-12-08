using BLL.Interfaces;
using DAL.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using JustChat.DAL.ViewModel;
using Microsoft.AspNetCore.SignalR;
using JustChat.BLL.Hubs;
using JustChat.DAL.Entities;
using JustChat.BLL.Interfaces;
using System.Text;

namespace JustChat.RbbitMQ.Consumer
{
    public class RabbitMQConsumer : BackgroundService

    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;
        private readonly IHubContext<ChatHub> _hubContext;

        public RabbitMQConsumer(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<RabbitMQConsumer>();
            _scopeFactory = scopeFactory;
            _messageService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMessageService>();
            _hubContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();
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

                var mesageData = JsonConvert.DeserializeObject<MessageRequest>(message);

                //Message newMessage = new Message
                //{
                //    MessageId = mesageData.MessageId,
                //    UserName = mesageData.UserName,
                //    Text = mesageData.Text,
                //    PublishDate = mesageData.PublishDate
                //};

                //await _messageService.AddMessageAsync(newMessage);

                //var newFile = new MetaFile();
                //if (mesageData.Image != null)
                //{
                //    newFile = await _fileService.PostFileAsync(mesageData);
                //}

                //var response = new MessageResponse
                //{
                //    message = newMessage,
                //    rsp = mesageData.Image != null ? newFile : null

                //};

                //await _hubContext.Clients.All.SendAsync("SendMessage", response);

                Console.WriteLine(ea.Body);
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

