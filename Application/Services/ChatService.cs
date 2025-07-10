using Application.DTO;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatRepository repository, ILogger<ChatService> logger)
        {
            _repository = repository;
            _logger = logger;
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
                                    CreatedAt = x.CreatedAt
                                }).ToHashSet()
            };

            return chatDTO;
        }
    }
}
