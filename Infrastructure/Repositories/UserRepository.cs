using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ChatDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<User> GetById(Guid id)
        {
            try
            {
                return await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while geting user with id: '{id}'");
                throw;
            }
        }

        public async Task<User> GetByUsername(string username)
        {
            try
            {
                return await _dbContext.User.FirstOrDefaultAsync(x => x.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while geting user with name: '{username}'");
                throw;
            }
        }

        public async Task Add(User user)
        {
            try
            {
                await _dbContext.User.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while adding user with username: '{user.Username}'");
                throw;
            }
        }
    }
}
