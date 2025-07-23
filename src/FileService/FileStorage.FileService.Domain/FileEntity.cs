using FileStorage.Shared;

namespace FileStorage.FileService.Domain
{
    public class FileEntity : BaseEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; } // User ID from Identity Service
        public List<FileShare> Shares { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}