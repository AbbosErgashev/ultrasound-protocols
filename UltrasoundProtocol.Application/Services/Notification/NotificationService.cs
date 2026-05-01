using AutoMapper;
using UltrasoundProtocol.Application.DTOs.Notification;
using UltrasoundProtocol.Domain.Enums;
using UltrasoundProtocol.Domain.Interfaces;
using NotificationEntity = UltrasoundProtocol.Domain.Entities.Notification;

namespace UltrasoundProtocol.Application.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications
            .FindAsync(n => n.UserId == userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(
            notifications.OrderByDescending(n => n.SentAt));
    }

    public async Task<IEnumerable<NotificationDto>> GetByUsernameAsync(string username)
    {
        var notifications = await _unitOfWork.Notifications
            .FindAsync(n => n.RecipientUsername == username);
        return _mapper.Map<IEnumerable<NotificationDto>>(
            notifications.OrderByDescending(n => n.SentAt));
    }

    public async Task SendAsync(
        Guid? userId, string? recipientUsername,
        NotificationType type, string title, string message)
    {
        var notification = new NotificationEntity
        {
            UserId = userId,
            RecipientUsername = recipientUsername,
            Type = type,
            Title = title,
            Message = message,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification is null) return;

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid? userId, string? username)
    {
        IEnumerable<NotificationEntity> notifications;

        if (userId.HasValue)
            notifications = await _unitOfWork.Notifications
                .FindAsync(n => n.UserId == userId && !n.IsRead);
        else
            notifications = await _unitOfWork.Notifications
                .FindAsync(n => n.RecipientUsername == username && !n.IsRead);

        return notifications.Count();
    }
}
