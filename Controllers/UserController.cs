using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Dtos;
using UserApi.Models;
using UserApi.Services;

namespace UserApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var users = await _userService.GetAllAsync(page, pageSize);
            var total = await _userService.GetCountAsync();

            return Ok(new
            {
                users,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "User id is required"
                });
            }

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser(
            [FromBody] UserCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Invalid user data"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return BadRequest(new
                {
                    message = "Username is required"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required"
                });
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var existingUser = await _userService.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return Conflict(new
                {
                    message = "Email is already registered"
                });
            }

            var password = string.IsNullOrWhiteSpace(dto.Password)
                ? "Temp@12345"
                : dto.Password.Trim();

            if (password.Length < 8)
            {
                return BadRequest(new
                {
                    message = "Password must be at least 8 characters"
                });
            }

            var role = dto.Role?.Trim();

            if (role != "Student" && role != "Employee")
            {
                role = "Student";
            }

            var status = dto.Status?.Trim();

            if (status != "Active" && status != "Inactive")
            {
                status = "Active";
            }

            var user = new User
            {
                Username = dto.Username.Trim(),
                Email = email,
                Role = role,
                Status = status,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                ResetToken = null,
                ResetTokenExpiry = null
            };

            await _userService.CreateAsync(user);

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                new
                {
                    message = "User created successfully",
                    userId = user.Id,
                    temporaryPassword = string.IsNullOrWhiteSpace(dto.Password)
                        ? password
                        : null
                }
            );
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            string id,
            [FromBody] UserUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "User id is required"
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Invalid user data"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    message = "Username and email are required"
                });
            }

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var userWithSameEmail = await _userService.GetByEmailAsync(email);

            if (userWithSameEmail != null && userWithSameEmail.Id != id)
            {
                return Conflict(new
                {
                    message = "Another user already uses this email"
                });
            }

            var role = dto.Role?.Trim();

            if (role != "Student" && role != "Employee")
            {
                role = "Student";
            }

            var status = dto.Status?.Trim();

            if (status != "Active" && status != "Inactive")
            {
                status = "Active";
            }

            // Keep the existing PasswordHash so the user can still log in.
            existingUser.Username = dto.Username.Trim();
            existingUser.Email = email;
            existingUser.Role = role;
            existingUser.Status = status;

            await _userService.UpdateAsync(id, existingUser);

            return Ok(new
            {
                message = "User updated successfully"
            });
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "User id is required"
                });
            }

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }

            await _userService.DeleteAsync(id);

            return Ok(new
            {
                message = "User deleted successfully"
            });
        }
    }
}