using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Protocol;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "User")]
public class MyDiagnosesController : Controller
{
    private readonly IProtocolService _protocolService;

    public MyDiagnosesController(IProtocolService protocolService)
    {
        _protocolService = protocolService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return View(Enumerable.Empty<UltrasoundProtocol.Application.DTOs.Protocol.ProtocolDto>());

        var protocols = await _protocolService.GetByPatientIdAsync(userId);
        return View(protocols);
    }
}
