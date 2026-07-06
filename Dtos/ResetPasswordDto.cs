using System.ComponentModel;

namespace UserApi.Dtos
{
    public class ResetPasswordDto

{
    [DefaultValue("")]  
    public required string ResetToken { get; set; }
    [DefaultValue("")]

    public required string NewPassword { get; set; }
    [DefaultValue("")]

    public required string ConfirmPassword { get; set; }
}
}