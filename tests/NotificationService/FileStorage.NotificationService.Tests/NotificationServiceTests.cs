using Moq;
using Microsoft.AspNetCore.SignalR;
using FileStorage.NotificationService.API.Hubs;

namespace FileStorage.NotificationService.Tests
{
    public class NotificationServiceTests
    {
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IGroupManager> _mockGroups;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockGroups = new Mock<IGroupManager>();

            _mockHubContext.Setup(x => x.Clients).Returns(_mockClients.Object);
            _mockClients.Setup(x => x.Group(It.IsAny<string>())).Returns(_mockClients.Object);
            _mockHubContext.Setup(x => x.Groups).Returns(_mockGroups.Object);

            _notificationService = new NotificationService(_mockHubContext.Object);
        }

        [Fact]
        public async Task SendFileShareNotificationAsync_ShouldCallSendAsyncOnGroup()
        {
            // Arrange
            var userId = 1;
            var message = "Test share notification";

            // Act
            await _notificationService.SendFileShareNotificationAsync(userId, message);

            // Assert
            _mockClients.Verify(x => x.Group($"user_{userId}"), Times.Once);
            _mockClients.Verify(x => x.SendAsync("FileShareAccepted", It.IsAny<object>(), default), Times.Once);
        }

        [Fact]
        public async Task SendCommentNotificationAsync_ShouldCallSendAsyncOnGroup()
        {
            // Arrange
            var userId = 1;
            var message = "Test comment notification";

            // Act
            await _notificationService.SendCommentNotificationAsync(userId, message);

            // Assert
            _mockClients.Verify(x => x.Group($"user_{userId}"), Times.Once);
            _mockClients.Verify(x => x.SendAsync("NewComment", It.IsAny<object>(), default), Times.Once);
        }
    }
}