namespace FileStorage.FileService.Application.DTOs
{
    public class SharedFileDto
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? SharedBy { get; set; } // Username of the sharer
        public DateTime SharedAt { get; set; }
        public string? ShareToken { get; set; }
        public bool IsAccepted { get; set; }
    }
}