using FileStorage.Shared;
using System;

namespace FileStorage.FileService.Domain
{
    public class FileShare : BaseEntity
    {
        public int FileId { get; set; }
        public FileEntity File { get; set; }
        public string SharedWithEmail { get; set; }
        public string ShareToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public bool IsAccepted { get; set; }
        public SharePermission Permission { get; set; } = SharePermission.Read;
    }

    public enum SharePermission { Read, ReadWrite }
}