using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        // GET: api/profile/me
        [HttpGet("")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            var email = User.FindFirst(
                ClaimTypes.Email
            )?.Value;

            var role = User.FindFirst(
                ClaimTypes.Role
            )?.Value;

            return Ok(new
            {
                Id = userId,
                Email = email,
                Role = role,
                Message = "Profile retrieved successfully"
            });
        }


        // PUT: api/profile/update
        [HttpPut("update")]
        public IActionResult UpdateProfile()
        {
            return Ok(new
            {
                Message = "Profile updated successfully"
            });
        }


        // PUT: api/profile/change-password
        [HttpPut("change-password")]
        public IActionResult ChangePassword()
        {
            return Ok(new
            {
                Message = "Password changed successfully"
            });
        }
    }
}