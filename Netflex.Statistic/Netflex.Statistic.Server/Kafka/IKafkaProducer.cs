using Netflex.Statistic.Server.Models;

namespace Netflex.Statistic.Server.Kafka
{
    public interface IKafkaProducer
    {
        Task<bool> Post(StatisticRequest orderRequest);
    }
}