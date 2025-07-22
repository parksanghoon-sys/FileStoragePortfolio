using FileStorage.Shared;
using FileStorage.FileService.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStorage.FileService.Application.Interfaces
{
    public interface IFileShareService
    {
        Task<ApiResponse<string>> CreateShareLinkAsync(int fileId, string email, int userId);
        Task<ApiResponse<bool>> AcceptShareAsync(string shareToken);
        Task<ApiResponse<bool>> RejectShareAsync(string shareToken);
        Task<ApiResponse<List<SharedFileDto>>> GetSharedFilesAsync(int userId);
    }
}