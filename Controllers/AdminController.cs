using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserApi.Constants;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: api/admin
        [HttpGet("")]
        public async Task<IActionResult> GetAdmin()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized();
            }

            var admin = await _adminService.GetByIdAsync(adminId);

            if (admin == null)
            {
                return NotFound(new
                {
                    Message = "Admin not found"
                });
            }

            return Ok(admin);
        }
    }
}