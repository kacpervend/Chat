using Application.DTO;
using Confluent.Kafka;
using Domain.Entities;
using Domain.Options;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MessageBroker
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly KafkaOption _kafkaOption;

        public KafkaConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<KafkaOption> options)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _kafkaOption = options.Value;   
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig()
            {
                GroupId = "Message",
                BootstrapServers = _kafkaOption.Url,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe("Message");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    var messageDTO = JsonSerializer.Deserialize<MessageDTO>(consumeResult.Message.Value);

                    var message = new Message()
                    {
                        MessageId = messageDTO!.MessageId,
                        ChatId = messageDTO!.ChatId,
                        SenderId = messageDTO!.SenderId,
                        CreatedAt = messageDTO!.CreatedAt,
                        MessageText = messageDTO!.MessageText
                    };

                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

                    await dbContext.Message.AddAsync(message);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
