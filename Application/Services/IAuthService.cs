using Application.DTO;

namespace Application.Services
{
    public interface IAuthService
    {
        Task RegisterUser(RegisterUserDTO dto);
        Task<AuthDTO> GenerateToken(LoginDTO dto);
    }
}
