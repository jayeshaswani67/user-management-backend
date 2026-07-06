using System.Security.Cryptography;
using UserApi.Dtos;
using UserApi.Models;

namespace UserApi.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        private readonly IWebHostEnvironment _environment;

        public AuthService(
            UserService userService,
            JwtService jwtService,
            IWebHostEnvironment environment)
        {
            _userService = userService;
            _jwtService = jwtService;
            _environment = environment;
        }

        // POST: /api/auth/register
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (dto == null)
                throw new Exception("Invalid registration request.");

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new Exception("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Password is required.");

            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                throw new Exception("Confirm password is required.");

            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password and confirm password do not match.");

            if (dto.Password.Length < 8)
                throw new Exception("Password must be at least 8 characters.");

            var email = dto.Email.Trim().ToLowerInvariant();
            var username = dto.Username.Trim();

            var existingEmailUser = await _userService.GetByEmailAsync(email);

            if (existingEmailUser != null)
                throw new Exception("This email is already registered.");

            var existingUsernameUser =
                await _userService.GetByUsernameAsync(username);

            if (existingUsernameUser != null)
                throw new Exception("This username is already taken.");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Student",
                Status = "Active",
                ResetToken = null,
                ResetTokenExpiry = null
            };

            await _userService.CreateAsync(user);

            var token = _jwtService.GenerateToken(
                user.Id!,
                user.Email,
                user.Role
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                UserId = user.Id!,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        // POST: /api/auth/login
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            if (dto == null)
                throw new Exception("Invalid request.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Password is required.");

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("Invalid email or password.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new Exception(
                    "This account has no valid password. Delete this old user and register again."
                );
            }

            bool passwordMatches;

            try
            {
                passwordMatches = BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash
                );
            }
            catch
            {
                throw new Exception(
                    "This account has an invalid password hash. Delete this old user and register again."
                );
            }

            if (!passwordMatches)
                throw new Exception("Invalid email or password.");

            if (user.Status == "Inactive")
                throw new Exception("This account is inactive.");

            var role = string.IsNullOrWhiteSpace(user.Role)
                ? "Student"
                : user.Role;

            var token = _jwtService.GenerateToken(
                user.Id!,
                user.Email,
                role
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                UserId = user.Id!,
                Username = user.Username,
                Email = user.Email,
                Role = role
            };
        }


    public async Task<AuthResponseDto> ForgotPassword(ForgotPasswordDto dto)
{
    if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
    {
        throw new Exception("Email is required.");
    }

    var email = dto.Email.Trim().ToLowerInvariant();

    var user = await _userService.GetByEmailAsync(email);

    if (user == null || string.IsNullOrWhiteSpace(user.Id))
    {
        throw new Exception("Account not found.");
    }

    var resetToken = GenerateResetToken();
    var resetTokenExpiry = DateTime.UtcNow.AddMinutes(20);

    await _userService.SaveResetTokenAsync(
        user.Id,
        resetToken,
        resetTokenExpiry
    );


    Console.WriteLine($"RESET TOKEN FOR: {user.Email}");
    Console.WriteLine($"TOKEN: {resetToken}");
    Console.WriteLine($"EXPIRES AT UTC: {resetTokenExpiry:O}"); 

    return new AuthResponseDto
    {
        Success = true,
        Message = "Password reset token generated."
    };
}

// POST: /api/auth/reset-password
public async Task<AuthResponseDto> ResetPassword(ResetPasswordDto dto)
{
    if (dto == null)
        throw new Exception("Invalid request.");

    if (string.IsNullOrWhiteSpace(dto.ResetToken))
        throw new Exception("Reset token is required.");

    if (string.IsNullOrWhiteSpace(dto.NewPassword))
        throw new Exception("New password is required.");

    if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
        throw new Exception("Confirm password is required.");

    if (dto.NewPassword != dto.ConfirmPassword)
        throw new Exception("Passwords do not match.");

    if (dto.NewPassword.Length < 8)
        throw new Exception("Password must be at least 8 characters.");

    var resetToken = dto.ResetToken.Trim();

    var user = await _userService.GetByResetTokenAsync(resetToken);

    if (user == null)
    {
        throw new Exception(
            "Invalid reset token. Generate a new reset token and try again."
        );
    }

    if (user.ResetTokenExpiry == null)
    {
        throw new Exception("This reset token has no expiry time.");
    }

    if (user.ResetTokenExpiry.Value <= DateTime.UtcNow)
    {
        throw new Exception("Reset token has expired. Generate a new token.");
    }

    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

    await _userService.UpdatePasswordAndClearResetTokenAsync(
        user.Id!,
        passwordHash
    );

    return new AuthResponseDto
    {
        Success = true,
        Message = "Password reset successful."
    };
}
        private static string GenerateResetToken()
        {
            return Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32)
            );
        }
    }
}