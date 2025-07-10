using Domain.Entities;

namespace Domain.Repositories
{
    public interface IChatRepository
    {
        Task<Chat> GetChatWithMessages(string chatName, int pageNumber, int pageSize);
    }
}