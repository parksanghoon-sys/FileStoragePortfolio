using System;
using System.Collections.Generic;

namespace FileStorage.FileService.Application.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int FileId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; } // Username of the commenter
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }
}