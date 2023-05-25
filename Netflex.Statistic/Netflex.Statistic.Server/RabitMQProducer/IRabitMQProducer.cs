namespace Netflex.Statistic.Server.RabitMQProducer
{
    public interface IRabitMQProducer
    {
        Task SendStatistic(string request);
    }
}