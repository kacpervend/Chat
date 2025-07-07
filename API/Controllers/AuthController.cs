using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDTO dto)
        {
            try
            {
                await _authService.RegisterUser(dto);

                return Ok();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogWarning(invalidOperationException, $"Login: '{dto.Login}' is already taken!");
                return Conflict(invalidOperationException.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during registration of user with login: '{dto.Login}'");
                return StatusCode(500, "An unexpected error occurred during registration!");
            }
        }
    }
}
