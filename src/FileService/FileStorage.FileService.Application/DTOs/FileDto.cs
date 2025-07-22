using System;

namespace FileStorage.FileService.Application.DTOs
{
    public class FileDto
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}