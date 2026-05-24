using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Content;
using UltrasoundProtocol.Application.Services.Statistics;

namespace UltrasoundProtocol.API.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IStatisticsService _statisticsService;
    private readonly IContentService _contentService;

    public DashboardController(IStatisticsService statisticsService, IContentService contentService)
    {
        _statisticsService = statisticsService;
        _contentService = contentService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("ContentManager"))
        {
            ViewBag.News = await _contentService.GetAllNewsAsync();
            ViewBag.Services = await _contentService.GetAllServicesAsync();
            ViewBag.Doctors = await _contentService.GetAllDoctorsAsync();
            return View("ContentManagerDashboard");
        }

        var userIdClaim = User.FindFirst("UserId")?.Value;
        Guid? userId = Guid.TryParse(userIdClaim, out var uid) ? uid : null;
        var stats = await _statisticsService.GetDashboardAsync(User.Identity?.Name, userId);
        var services = await _contentService.GetActiveServicesAsync();
        ViewBag.Services = services.OrderBy(s => s.SortOrder).ThenBy(s => s.Name);
        return View(stats);
    }
}
