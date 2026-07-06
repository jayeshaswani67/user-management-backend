using UserApi.Dtos;

namespace UserApi.Services
{
    public class ProfileService
    {
        private readonly AdminService _adminService;
        private readonly UserService _UserService;
        

        public ProfileService(
            AdminService adminService,
            UserService UserService
        )
        {
            _adminService = adminService;
            _UserService = UserService;

        }

        public async Task<object> GetProfile(string id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin != null)
                return admin;

            var User = await _UserService.GetByIdAsync(id);
            if (User != null)
                return User;

            throw new Exception("Profile not found.");
        }

        public async Task UpdateProfile(
            string id,
            UpdateProfileDto dto)
        {
            // Admin
            var admin = await _adminService.GetByIdAsync(id);

            if (admin != null)
            {
                admin.Username = dto.username;
                admin.Email = dto.email;

                if (!string.IsNullOrWhiteSpace(dto.newPassword))
                {
                    if (!BCrypt.Net.BCrypt.Verify(dto.currentPassword, admin.PasswordHash))
                        throw new Exception("Current password is incorrect.");

                    admin.PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(dto.newPassword);
                }

                await _adminService.UpdateAsync(id, admin);
                return;
            }

            // User
            var User = await _UserService.GetByIdAsync(id);

            if (User != null)
            {
                User.Username = dto.username;
                User.Email = dto.email;

                if (!string.IsNullOrWhiteSpace(dto.newPassword))
                {
                    if (!BCrypt.Net.BCrypt.Verify(dto.currentPassword, User.PasswordHash))
                        throw new Exception("Current password is incorrect.");

                    User.PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(dto.newPassword);
                }

                await _UserService.UpdateAsync(id, User);
                return;
            }

            throw new Exception("Profile not found.");
        }
    }
}