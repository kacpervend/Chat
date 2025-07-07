using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetById(Guid id);
        Task<User> GetByLogin(string login);
        Task Add(User user);
    }
}
