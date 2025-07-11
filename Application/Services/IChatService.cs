using Application.DTO;

namespace Application.Services
{
    public interface IChatService
    {
        Task<ChatDTO> GetPaginatedChat(string chatName, int pageNumber, int pageSize);
        Task SaveMessage(MessageDTO messageDTO);
    }
}
