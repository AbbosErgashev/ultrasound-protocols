using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Notification;

namespace UltrasoundProtocol.API.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        var username = User.Identity?.Name;

        if (Guid.TryParse(userIdClaim, out var userId))
            return View(await _notificationService.GetByUserIdAsync(userId));

        if (!string.IsNullOrWhiteSpace(username))
            return View(await _notificationService.GetByUsernameAsync(username));

        return View(Enumerable.Empty<UltrasoundProtocol.Application.DTOs.Notification.NotificationDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id, string? returnUrl)
    {
        await _notificationService.MarkAsReadAsync(id);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }
}
