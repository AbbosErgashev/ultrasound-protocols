using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Pdf;
using UltrasoundProtocol.Application.Services.Protocol;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "User")]
public class MyDiagnosesController : Controller
{
    private readonly IProtocolService _protocolService;
    private readonly IPdfService _pdfService;
    private readonly IBreastProtocolService _breastProtocolService;

    public MyDiagnosesController(
        IProtocolService protocolService,
        IPdfService pdfService,
        IBreastProtocolService breastProtocolService)
    {
        _protocolService = protocolService;
        _pdfService = pdfService;
        _breastProtocolService = breastProtocolService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return View(Enumerable.Empty<UltrasoundProtocol.Application.DTOs.Protocol.ProtocolDto>());

        var protocols = await _protocolService.GetByPatientIdAsync(userId);
        return View(protocols);
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Forbid();

        var protocol = await _protocolService.GetByIdAsync(id);
        if (protocol is null)
            return NotFound();

        if (protocol.PatientId != userId)
            return Forbid();

        var breastProtocol = IsBreastProtocol(protocol.BodyPart)
            ? await _breastProtocolService.GetPdfDataByExamIdAsync(id)
            : null;

        var pdf = breastProtocol is null
            ? _pdfService.GenerateProtocolPdf(protocol)
            : _pdfService.GenerateBreastProtocolPdf(protocol, breastProtocol);

        var fileName = $"UZI_Tashxis_{MakeSafeFileName(protocol.BodyPart)}_{protocol.ExamDate:yyyyMMdd}.pdf";
        return File(pdf, "application/pdf", fileName);
    }

    private static bool IsBreastProtocol(string bodyPart) =>
        bodyPart.Contains("Sut bez", StringComparison.OrdinalIgnoreCase);

    private static string MakeSafeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(safe) ? "Protokol" : safe;
    }
}
