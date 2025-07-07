using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository repository, IMapper mapper, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task RegisterUser(RegisterUserDTO dto)
        {
            try
            {
                var existingLogin = await _repository.GetByLogin(dto.Login);

                if (existingLogin != null)
                {
                    _logger.LogWarning($"Login: '{dto.Login}' is already taken!");
                    throw new InvalidOperationException($"Login: '{dto.Login}' is already taken!");
                }

                var user = _mapper.Map<User>(dto);

                var hashedPassword = _passwordHasher.HashPassword(user, user.Password);

                user.CreatedAt = DateTime.Now;
                user.Password = hashedPassword;

                await _repository.Add(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user with login: '{dto.Login}'");
                throw;
            }
        }
    }
}
