using FileStorage.IdentityService.Application.DTOs;
using FileStorage.IdentityService.Application.Interfaces;
using FileStorage.IdentityService.Domain;
using FileStorage.Shared;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FileStorage.IdentityService.Infrastructure.Repositories;

namespace FileStorage.IdentityService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;

        public AuthService(IConfiguration config, IRepository<User> userRepository, IRepository<RefreshToken> refreshTokenRepository)
        {
            _config = config;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ApiResponse<AuthResult>> LoginAsync(LoginRequest request)
        {
            var user = await (await _userRepository.GetQueryable())
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash))
            {
                return new ApiResponse<AuthResult> { Success = false, Message = "Invalid credentials." };
            }

            var authResult = GenerateAuthResult(user);
            return new ApiResponse<AuthResult> { Success = true, Data = authResult };
        }

        public async Task<ApiResponse<AuthResult>> RegisterAsync(RegisterRequest request)
        {
            if (await (await _userRepository.GetQueryable()).AnyAsync(u => u.Email == request.Email))
            {
                return new ApiResponse<AuthResult> { Success = false, Message = "User with this email already exists." };
            }

            CreatePasswordHash(request.Password, out string passwordHash);

            var user = new User
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = UserRole.User, // Default role
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var authResult = GenerateAuthResult(user);
            return new ApiResponse<AuthResult> { Success = true, Data = authResult };
        }

        public async Task<ApiResponse<AuthResult>> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await (await _refreshTokenRepository.GetQueryable())
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
            {
                return new ApiResponse<AuthResult> { Success = false, Message = "Invalid refresh token." };
            }

            // Revoke old token
            storedToken.Revoked = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            var newAuthResult = GenerateAuthResult(storedToken.User);
            return new ApiResponse<AuthResult> { Success = true, Data = newAuthResult };
        }

        public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await (await _refreshTokenRepository.GetQueryable())
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "Refresh token not found." };
            }

            storedToken.Revoked = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            return new ApiResponse<bool> { Success = true, Data = true };
        }

        private AuthResult GenerateAuthResult(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:AccessTokenExpirationMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationDays"])),
                Created = DateTime.UtcNow,
                UserId = user.Id
            };

            // Add new refresh token to user's collection and save
            user.RefreshTokens.Add(refreshToken);
            _userRepository.UpdateAsync(user).Wait(); // Sync call for simplicity, consider async

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Username = user.Username,
                Email = user.Email
            };
        }

        private void CreatePasswordHash(string password, out string passwordHash)
        {
            passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}