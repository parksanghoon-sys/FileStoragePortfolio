using FileStorage.Shared;
using FileStorage.IdentityService.Application.DTOs;

namespace FileStorage.IdentityService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResult>> LoginAsync(LoginRequest request);
        Task<ApiResponse<AuthResult>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResult>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken);
    }
}