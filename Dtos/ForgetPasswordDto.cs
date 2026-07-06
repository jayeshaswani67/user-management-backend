using System.ComponentModel;

namespace UserApi.Dtos
{
    public class ForgotPasswordDto
    {
        [DefaultValue("")]
        public string Email { get; set; }
            = string.Empty;
    }
}