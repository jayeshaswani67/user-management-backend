using Microsoft.AspNetCore.Mvc;
using UserApi.Dtos;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        

        // Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPassword(dto);
            return Ok(result);
        }

        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordDto dto)
        {
            var result = await _authService.ResetPassword(dto);
            return Ok(result);
        }
    }
}