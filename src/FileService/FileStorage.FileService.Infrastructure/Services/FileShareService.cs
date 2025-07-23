using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Application.Interfaces;
using FileStorage.FileService.Domain;
using FileStorage.Shared;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.FileService.Infrastructure.Services
{
    public class FileShareService : IFileShareService
    {
        private readonly IRepository<FileEntity> _fileRepository;
        private readonly IRepository<FileShare> _shareRepository;
        private readonly IEmailService _emailService;

        public FileShareService(IRepository<FileEntity> fileRepository, IRepository<FileShare> shareRepository, IEmailService emailService)
        {
            _fileRepository = fileRepository;
            _shareRepository = shareRepository;
            _emailService = emailService;
        }

        public async Task<ApiResponse<string>> CreateShareLinkAsync(int fileId, string email, int userId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null || file.UserId != userId)
                return new ApiResponse<string> { Success = false, Message = "File not found or unauthorized." };

            var shareToken = Guid.NewGuid().ToString();
            var share = new FileShare
            {
                FileId = fileId,
                SharedWithEmail = email,
                ShareToken = shareToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _shareRepository.AddAsync(share);

            await _emailService.SendShareInvitationAsync(email, shareToken, file.FileName);

            return new ApiResponse<string> { Success = true, Data = shareToken };
        }

        public async Task<ApiResponse<bool>> AcceptShareAsync(string shareToken)
        {
            var share = await (await _shareRepository.GetQueryable())
                .FirstOrDefaultAsync(s => s.ShareToken == shareToken && !s.IsAccepted && s.ExpiresAt > DateTime.UtcNow);

            if (share == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "Invalid or expired share token." };
            }

            share.IsAccepted = true;
            share.AcceptedAt = DateTime.UtcNow;
            await _shareRepository.UpdateAsync(share);

            return new ApiResponse<bool> { Success = true, Data = true };
        }

        public async Task<ApiResponse<bool>> RejectShareAsync(string shareToken)
        {
            var share = await (await _shareRepository.GetQueryable())
                .FirstOrDefaultAsync(s => s.ShareToken == shareToken && !s.IsAccepted && s.ExpiresAt > DateTime.UtcNow);

            if (share == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "Invalid or expired share token." };
            }

            await _shareRepository.DeleteAsync(share.Id);

            return new ApiResponse<bool> { Success = true, Data = true };
        }

        public async Task<ApiResponse<List<SharedFileDto>>> GetSharedFilesAsync(int userId)
        {
            // TODO: 사용자 ID로 공유된 파일 조회 로직 구현
            // 현재는 UserId가 FileEntity에만 있으므로, FileShare에 SharedWithUserId를 추가하거나
            // Identity Service에서 사용자 이메일을 통해 UserId를 가져와야 함.
            // 임시로 빈 리스트 반환
            return new ApiResponse<List<SharedFileDto>> { Success = true, Data = new List<SharedFileDto>() };
        }
    }
}