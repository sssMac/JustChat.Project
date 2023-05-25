﻿using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Netflex.Statistic.Server.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Netflex.Statistic.Server.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "statistics";

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
            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
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
    }

}