using FileStorage.Shared;
using System.Collections.Generic;

namespace FileStorage.FileService.Domain
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public int FileId { get; set; }
        public FileEntity File { get; set; }
        public int UserId { get; set; } // User ID from Identity Service
        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public List<Comment> Replies { get; set; } = new();
    }
}