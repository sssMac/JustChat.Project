
namespace Kafka.Producer
{
    public interface IKafkaProducer
    {
        Task<bool> Post(StatisticRequest orderRequest);
    }
}