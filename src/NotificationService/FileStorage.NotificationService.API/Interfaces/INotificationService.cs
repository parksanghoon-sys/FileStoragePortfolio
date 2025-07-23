namespace FileStorage.NotificationService.API.Interfaces
{
    public interface INotificationService
    {
        Task SendFileShareNotificationAsync(int userId, string message);
        Task SendCommentNotificationAsync(int userId, string message);
    }
}