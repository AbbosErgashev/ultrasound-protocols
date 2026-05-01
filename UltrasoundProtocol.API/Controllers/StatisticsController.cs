using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Statistics;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "SeniorDoctor,ChiefDoctor")]
public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        Guid? userId = Guid.TryParse(userIdClaim, out var uid) ? uid : null;
        var stats = await _statisticsService.GetDashboardAsync(User.Identity?.Name, userId);
        return View(stats);
    }
}
