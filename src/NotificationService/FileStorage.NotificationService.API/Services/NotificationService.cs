using FileStorage.NotificationService.API.Interfaces;
using FileStorage.NotificationService.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FileStorage.NotificationService.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendFileShareNotificationAsync(int userId, string message)
        {
            await _hubContext.Clients.Group($"user_{userId}")
                .SendAsync("FileShareAccepted", new { message, timestamp = DateTime.UtcNow });
        }

        public async Task SendCommentNotificationAsync(int userId, string message)
        {
            await _hubContext.Clients.Group($"user_{userId}")
                .SendAsync("NewComment", new { message, timestamp = DateTime.UtcNow });
        }
    }
}