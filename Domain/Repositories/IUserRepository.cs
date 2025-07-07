using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetById(Guid id);
        Task<User> GetByUsername(string username);
        Task Add(User user);
    }
}
