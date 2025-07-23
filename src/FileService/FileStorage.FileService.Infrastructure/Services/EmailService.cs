using FileStorage.FileService.Application.Interfaces;

namespace FileStorage.FileService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public Task SendShareInvitationAsync(string toEmail, string shareToken, string fileName)
        {
            Console.WriteLine($"Sending share invitation to {toEmail} for file {fileName} with token {shareToken}");
            // 실제 이메일 전송 로직 (SendGrid, Mailgun 등 사용 예정)
            return Task.CompletedTask;
        }
    }
}