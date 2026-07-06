namespace UserApi.Dtos
{
    public class AuthResponseDto
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }

         public string Token { get; set; } = "";
        public  string UserId { get; set; }
        public  string Email { get; set; }
        public  string Role { get; set; }
        public  string Username { get; set; }
        public string? ResetToken { get; set; }
    }
}