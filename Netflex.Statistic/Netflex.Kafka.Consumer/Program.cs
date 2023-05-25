// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Netflex.Kafka.Consumer;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var config = new ConsumerConfig
{
    GroupId = "consumer-group",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    AllowAutoCreateTopics = true,
};


using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
consumer.Subscribe("topic");

CancellationTokenSource cts = new CancellationTokenSource();

try
{
    while (true)
    {
        var response = consumer.Consume(cts.Token);
        if (response.Message != null)
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