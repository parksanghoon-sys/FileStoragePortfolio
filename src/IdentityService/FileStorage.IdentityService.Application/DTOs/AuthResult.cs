namespace FileStorage.IdentityService.Application.DTOs
{
    public class AuthResult
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}