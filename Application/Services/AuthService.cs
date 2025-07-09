using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository repository, IMapper mapper, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task RegisterUser(RegisterUserDTO dto)
        {
            try
            {
                var existingLogin = await _repository.GetByUsername(dto.Username);

                if (existingLogin != null)
                {
                    _logger.LogWarning($"Username: '{dto.Username}' is already taken!");
                    throw new InvalidOperationException($"Username: '{dto.Username}' is already taken!");
                }

                var user = _mapper.Map<User>(dto);

                var hashedPassword = _passwordHasher.HashPassword(user, user.Password);

                user.CreatedAt = DateTime.Now;
                user.Password = hashedPassword;

                await _repository.Add(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user with name: '{dto.Username}'");
                throw;
            }
        }

        public async Task<AuthDTO> GenerateToken(LoginDTO loginDTO)
        {
            var user = await _repository.GetByUsername(loginDTO.Username);

            if (user == null)
                throw new InvalidOperationException("User with provided login doesn't exist!");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid username or password!");

            var expires = DateTime.Now.AddMinutes(int.Parse(_configuration["JwtSettings:ExpireMinutes"]));

            var token = _repository.GenerateToken(user);

            return new AuthDTO()
            {
                Token = token,
                ExpiredDate = expires
            };
        }
    }
}
