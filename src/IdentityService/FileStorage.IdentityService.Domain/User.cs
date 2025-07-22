using FileStorage.Shared;
using System.Collections.Generic;

namespace FileStorage.IdentityService.Domain
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }

    public enum UserRole { Admin, User, Guest }

    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}