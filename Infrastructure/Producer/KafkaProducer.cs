using Confluent.Kafka;
using Domain.Options;
using Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Infrastructure.Producer
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(IOptions<KafkaOption> options)
        {
            var kafka = options.Value;

            var config = new ConsumerConfig()
            {
                GroupId = "Message",
                BootstrapServers = kafka.Url,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task Produce(string topic, Message<string, string> message)
        {
            await _producer.ProduceAsync(topic, message);
        }
    }
}
