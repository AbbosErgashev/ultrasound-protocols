using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Audit;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "ChiefDoctor")]
public class AuditLogsController : Controller
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _auditService.GetAllAsync();
        return View(logs);
    }
}
