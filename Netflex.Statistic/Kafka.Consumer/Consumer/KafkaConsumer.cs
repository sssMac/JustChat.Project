using Confluent.Kafka;
using Kafka.Consumer.Configuration;
using Kafka.Consumer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Kafka.Consumer.Consumer
{
    public class KafkaConsumer : IHostedService
    {
        private readonly string topic = "statistics";
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
            await Task.Run(() => Start(cancellationToken));

        }

        private async Task Start(CancellationToken cancellationToken)
        {
            Console.WriteLine($"KafkaConsumer START!");

            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumerBuilder.Subscribe(topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var cr = consumerBuilder.Consume(cts.Token);
                    Console.WriteLine($"Consumed message '{cr.Message}' at '{cr.TopicPartitionOffset}'");
                }
            }
            //var statisticRequest = JsonSerializer.Deserialize<StatisticProcessingRequest>(consumer.Message.Value);
            //Console.WriteLine($" ----- > {statisticRequest.Name} FROM KafkaConsumer");
            //if (statisticRequest != null)
            //            {
            //                var entity = await _statistics.Find(s => s.ContentId == statisticRequest.Id).FirstAsync();
            //                if (entity == null)
            //                {
            //                    await _statistics.InsertOneAsync(new Statistic
            //                    {
            //                        ContentId = statisticRequest.Id,
            //                        ContentName = statisticRequest.Name,
            //                        ViewCount = 1
            //                    });
            //                }
            //                else
            //                {
            //                    var filter = Builders<Statistic>.Filter.Where(s => s.ContentId == statisticRequest.Id);
            //                    var update = Builders<Statistic>.Update.Set(x => x.ViewCount, entity.ViewCount + 1);
            //                    await _statistics.UpdateOneAsync(filter, update);
            //                }
            //            }
           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
