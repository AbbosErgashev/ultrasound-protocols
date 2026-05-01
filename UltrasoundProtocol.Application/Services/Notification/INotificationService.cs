using UltrasoundProtocol.Application.DTOs.Notification;
using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.Services.Notification;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<NotificationDto>> GetByUsernameAsync(string username);
    Task SendAsync(Guid? userId, string? recipientUsername, NotificationType type, string title, string message);
    Task MarkAsReadAsync(Guid notificationId);
    Task<int> GetUnreadCountAsync(Guid? userId, string? username);
}
