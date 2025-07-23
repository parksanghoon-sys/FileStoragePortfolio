using FileStorage.Shared;
using FileStorage.FileService.Application.DTOs;

namespace FileStorage.FileService.Application.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<CommentDto>> AddCommentAsync(AddCommentRequest request, int userId);
        Task<ApiResponse<List<CommentDto>>> GetFileCommentsAsync(int fileId);
        Task<ApiResponse<bool>> DeleteCommentAsync(int commentId, int userId);
    }
}