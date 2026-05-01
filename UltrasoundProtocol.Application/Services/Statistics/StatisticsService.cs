using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Statistics;
using UltrasoundProtocol.Domain.Enums;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(IUnitOfWork unitOfWork, ILogger<StatisticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardDto> GetDashboardAsync(string? username, Guid? userId)
    {
        _logger.LogDebug("Dashboard statistikasi so'raldi: {Username}", username);
        var allPatients = await _unitOfWork.Users
            .FindAsync(u => u.Role == UserRole.User);
        var allProtocols = await _unitOfWork.UltrasoundExams.GetAllAsync();
        var todayAppointments = await _unitOfWork.Appointments
            .FindAsync(a => a.AppointmentDate.Date == DateTime.UtcNow.Date);

        int unread = 0;
        if (userId.HasValue)
        {
            var notifications = await _unitOfWork.Notifications
                .FindAsync(n => n.UserId == userId && !n.IsRead);
            unread = notifications.Count();
        }
        else if (!string.IsNullOrEmpty(username))
        {
            var notifications = await _unitOfWork.Notifications
                .FindAsync(n => n.RecipientUsername == username && !n.IsRead);
            unread = notifications.Count();
        }

        var protocolList = allProtocols.ToList();
        var monthlyStats = protocolList
            .Where(p => p.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(p => p.CreatedAt.ToString("yyyy-MM"))
            .Select(g => new MonthlyStatDto
            {
                Month = g.Key,
                ProtocolCount = g.Count(),
                PatientCount = g.Select(p => p.PatientId).Distinct().Count()
            })
            .OrderBy(m => m.Month)
            .ToList();

        return new DashboardDto
        {
            TotalPatients = allPatients.Count(),
            TotalProtocols = protocolList.Count,
            ActiveProtocols = protocolList.Count(p => p.Status == ProtocolStatus.Active),
            CompletedProtocols = protocolList.Count(p => p.Status == ProtocolStatus.Completed),
            TodayAppointments = todayAppointments.Count(),
            UnreadNotifications = unread,
            MonthlyStats = monthlyStats
        };
    }
}
