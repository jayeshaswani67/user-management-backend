using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserApi.Dtos
{
    public class UserCreateDto
    {
        [Required]
        [MinLength(3)]
        [DefaultValue("")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [DefaultValue("")]
        public required string Email { get; set; }

        public string? Password { get; set; }

        [Required]
        [DefaultValue("")]
        public string Role { get; set; } = "Student";

        public string Status { get; set; } = "Active";
    }
}