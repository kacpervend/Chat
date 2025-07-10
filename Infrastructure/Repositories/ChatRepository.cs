using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Exceptions;

namespace Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(ChatDbContext dbContext, ILogger<ChatRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<Chat> GetChatWithMessages(string chatName, int pageNumber, int pageSize)
        {
            try
            {
                var chat = await _context.Chat
                                    .Where(x => x.Name == chatName)
                                    .Include(x => 
                                        x.Messages
                                            .OrderByDescending(x => x.CreatedAt)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                    )
                                    .ThenInclude(x => x.Sender)
                                    .FirstOrDefaultAsync();
                if (chat == null) 
                    throw new ChatNotFoundException(chatName);

                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error uccurred while fetching the chat with name: '{chatName}'");
                throw;
            }
        }
    }
}
