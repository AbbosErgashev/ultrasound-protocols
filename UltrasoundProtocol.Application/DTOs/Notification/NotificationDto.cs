using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? RecipientUsername { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}
