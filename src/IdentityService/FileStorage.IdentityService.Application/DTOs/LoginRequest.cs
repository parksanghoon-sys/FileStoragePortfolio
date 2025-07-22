namespace FileStorage.IdentityService.Application.DTOs
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}