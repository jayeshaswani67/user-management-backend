using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserApi.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3)]
        [MaxLength(50)]
        [DefaultValue("")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DefaultValue("")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must at least 8 characters")]
        [MaxLength(100)]
        [DefaultValue("")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is requied")]
        [DefaultValue("")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}