
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserApi.Dtos
{
    public class UpdateProfileDto
    {
        public required string username { get; set; }

        public required string email { get; set; }

        public string? currentPassword { get; set; }

        public string? newPassword { get; set; }
    }
}

