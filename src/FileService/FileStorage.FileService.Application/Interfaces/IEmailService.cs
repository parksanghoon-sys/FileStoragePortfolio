namespace FileStorage.FileService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendShareInvitationAsync(string toEmail, string shareToken, string fileName);
    }
}