using FileStorage.Shared;
using FileStorage.FileService.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FileStorage.FileService.Application.Interfaces
{
    public interface IFileService
    {
        Task<ApiResponse<FileUploadResult>> UploadFilesAsync(IFormFileCollection files, int userId);
        Task<ApiResponse<Stream>> DownloadFileAsync(int fileId, int userId);
        Task<ApiResponse<bool>> DeleteFileAsync(int fileId, int userId);
        Task<ApiResponse<PagedResult<FileDto>>> GetUserFilesAsync(int userId, int page, int size);
    }
}