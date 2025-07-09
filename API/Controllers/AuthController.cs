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

        [HttpPut("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDTO dto)
        {
            try
            {
                await _authService.RegisterUser(dto);

                return Ok();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogWarning(invalidOperationException, $"Username: '{dto.Username}' is already taken!");
                return Conflict(invalidOperationException.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during registration of user with name: '{dto.Username}'");
                return StatusCode(500, "An unexpected error occurred during registration!");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var authData = await _authService.GenerateToken(dto);

                return Ok(authData);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during login for user: '{dto.Username}'");
                return StatusCode(500, "An unexpected error occurred!");
            }
        }
    }
}
