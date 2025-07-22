namespace FileStorage.FileService.Application.DTOs
{
    public class AddCommentRequest
    {
        public string? Content { get; set; }
        public int FileId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}