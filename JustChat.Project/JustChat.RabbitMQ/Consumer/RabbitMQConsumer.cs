using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using JustChat.DAL.ViewModel;
using System.Text;

namespace JustChat.RbbitMQ.Consumer
{
    public class RabbitMQConsumer : BackgroundService

    {
        //private readonly ILogger _logger;
        //private readonly IMessageService _messageService;
        //private readonly IFileService _fileService;
        private readonly IConfiguration _config;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;

        public RabbitMQConsumer(
            IConfiguration config
            //ILoggerFactory loggerFactory, 
            //IMessageService messageService,
            //IFileService fileService
            )
        {
            //_logger = loggerFactory.CreateLogger<RabbitMQConsumer>();
            //_messageService = messageService;
            //_fileService = fileService;
            _config = config;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Hostname"],
                Port = Convert.ToInt32(_config["RabbitMQ:Port"]),
            };

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
            //_logger.LogInformation($"consumer received {content}");
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

