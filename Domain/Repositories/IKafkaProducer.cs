using Confluent.Kafka;

namespace Domain.Repositories
{
    public interface IKafkaProducer
    {
        Task Produce(string topic, Message<string, string> message);
    }
}