using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Application.Interfaces;
using FileStorage.FileService.Domain;
using FileStorage.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.FileService.Infrastructure.Repositories;

namespace FileStorage.FileService.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadPath;
        private readonly IRepository<FileEntity> _fileRepository;

        public FileService(IConfiguration configuration, IRepository<FileEntity> fileRepository)
        {
            _uploadPath = configuration["UploadPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(_uploadPath);
            _fileRepository = fileRepository;
        }

        public async Task<ApiResponse<FileUploadResult>> UploadFilesAsync(IFormFileCollection files, int userId)
        {
            var results = new List<FileDto>();

            foreach (var file in files)
            {
                if (!IsValidFile(file))
                    continue;

                var fileName = GenerateUniqueFileName(file.FileName);
                var filePath = Path.Combine(_uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fileEntity = new FileEntity
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _fileRepository.AddAsync(fileEntity);
                results.Add(new FileDto // 임시 DTO 생성
                {
                    Id = fileEntity.Id,
                    FileName = fileEntity.FileName,
                    FileSize = fileEntity.FileSize,
                    ContentType = fileEntity.ContentType,
                    UserId = fileEntity.UserId,
                    CreatedAt = fileEntity.CreatedAt
                });
            }

            return new ApiResponse<FileUploadResult>
            {
                Success = true,
                Data = new FileUploadResult { Files = results }
            };
        }

        public async Task<ApiResponse<Stream>> DownloadFileAsync(int fileId, int userId)
        {
            var fileEntity = await _fileRepository.GetByIdAsync(fileId);
            if (fileEntity == null || fileEntity.UserId != userId)
            {
                return new ApiResponse<Stream> { Success = false, Message = "File not found or unauthorized." };
            }

            var stream = new FileStream(fileEntity.FilePath, FileMode.Open, FileAccess.Read);
            return new ApiResponse<Stream> { Success = true, Data = stream };
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(int fileId, int userId)
        {
            var fileEntity = await _fileRepository.GetByIdAsync(fileId);
            if (fileEntity == null || fileEntity.UserId != userId)
            {
                return new ApiResponse<bool> { Success = false, Message = "File not found or unauthorized." };
            }

            File.Delete(fileEntity.FilePath);
            await _fileRepository.DeleteAsync(fileId);

            return new ApiResponse<bool> { Success = true, Data = true };
        }

        public async Task<ApiResponse<PagedResult<FileDto>>> GetUserFilesAsync(int userId, int page, int size)
        {
            var query = (await _fileRepository.GetQueryable())
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var pagedResult = new PagedResult<FileDto>
            {
                Items = items.Select(f => new FileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    ContentType = f.ContentType,
                    UserId = f.UserId,
                    CreatedAt = f.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = size
            };

            return new ApiResponse<PagedResult<FileDto>> { Success = true, Data = pagedResult };
        }

        private bool IsValidFile(IFormFile file)
        {
            return file != null && file.Length > 0;
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(originalFileName);
        }
    }
}