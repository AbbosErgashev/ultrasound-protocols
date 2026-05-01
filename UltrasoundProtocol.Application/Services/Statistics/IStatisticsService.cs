using UltrasoundProtocol.Application.DTOs.Statistics;

namespace UltrasoundProtocol.Application.Services.Statistics;

public interface IStatisticsService
{
    Task<DashboardDto> GetDashboardAsync(string? username, Guid? userId);
}
