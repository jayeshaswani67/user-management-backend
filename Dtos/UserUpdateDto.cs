
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserApi.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        [DefaultValue("")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DefaultValue("")]
        public string Email { get; set; } = string.Empty;


        [DefaultValue("")]
        public string? Password { get; set; }

        [DefaultValue("")]
         public string Role { get; set; } = "Student";

        public string Status { get; set; } = "Active";
    }
}
