using Application.DTO;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;
        private readonly IKafkaProducer _kafkaProducer;

        public ChatService(IChatRepository repository, IKafkaProducer kafkaProducer)
        {
            _repository = repository;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<ChatDTO> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
        {
            var chat = await _repository.GetChatWithMessages(chatName, pageNumber, pageSize);

            var chatDTO = new ChatDTO()
            {
                Id = chat.ChatId,
                Name = chat.Name,
                Messages = chat.Messages?
                                .OrderByDescending(x => x.CreatedAt)
                                .Select(x => new MessageDTO()
                                {
                                    MessageId = x.MessageId,
                                    Sender = x.Sender.Username,
                                    MessageText = x.MessageText,
                                    ChatId = chat.ChatId,
                                    SenderId = x.SenderId,
                                    CreatedAt = x.CreatedAt
                                }).ToHashSet()
            };

            return chatDTO;
        }

        public async Task SaveMessage(MessageDTO messageDTO)
        {
            await _kafkaProducer.Produce("Message", new Confluent.Kafka.Message<string, string>
            {
                Key = messageDTO.MessageId.ToString(),
                Value = JsonSerializer.Serialize(messageDTO)
            });
        }
    }
}
