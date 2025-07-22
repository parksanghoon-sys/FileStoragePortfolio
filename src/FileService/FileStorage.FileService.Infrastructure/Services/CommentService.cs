using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Application.Interfaces;
using FileStorage.FileService.Domain;
using FileStorage.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.FileService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.FileService.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<FileEntity> _fileRepository;
        // private readonly INotificationService _notificationService; // Notification Service에서 처리

        public CommentService(IRepository<Comment> commentRepository, IRepository<FileEntity> fileRepository /*, INotificationService notificationService */)
        {
            _commentRepository = commentRepository;
            _fileRepository = fileRepository;
            // _notificationService = notificationService;
        }

        public async Task<ApiResponse<CommentDto>> AddCommentAsync(AddCommentRequest request, int userId)
        {
            var file = await _fileRepository.GetByIdAsync(request.FileId);
            if (file == null) return new ApiResponse<CommentDto> { Success = false, Message = "File not found." };

            var comment = new Comment
            {
                Content = request.Content,
                FileId = request.FileId,
                UserId = userId,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);

            // TODO: 알림 전송은 Notification Service를 통해 처리
            // await _notificationService.SendCommentNotificationAsync(file.UserId, $"새로운 댓글이 파일 '{file.FileName}'에 달렸습니다.");

            return new ApiResponse<CommentDto> { Success = true, Data = new CommentDto { Id = comment.Id, Content = comment.Content, FileId = comment.FileId, UserId = comment.UserId, Username = "", CreatedAt = comment.CreatedAt } };
        }

        public async Task<ApiResponse<List<CommentDto>>> GetFileCommentsAsync(int fileId)
        {
            var comments = await (await _commentRepository.GetQueryable())
                .Where(c => c.FileId == fileId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return new ApiResponse<List<CommentDto>> { Success = true, Data = comments.Select(c => new CommentDto { Id = c.Id, Content = c.Content, FileId = c.FileId, UserId = c.UserId, Username = "", CreatedAt = c.CreatedAt }).ToList() };
        }

        public async Task<ApiResponse<bool>> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                return new ApiResponse<bool> { Success = false, Message = "Comment not found or unauthorized." };
            }

            await _commentRepository.DeleteAsync(commentId);

            return new ApiResponse<bool> { Success = true, Data = true };
        }
    }
}