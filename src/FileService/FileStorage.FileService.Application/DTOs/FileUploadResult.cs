namespace FileStorage.FileService.Application.DTOs
{
    public class FileUploadResult
    {
        public List<FileDto> Files { get; set; } = new();
    }
}