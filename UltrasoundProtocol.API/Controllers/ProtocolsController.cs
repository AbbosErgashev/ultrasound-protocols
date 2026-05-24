using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Content;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Excel;
using UltrasoundProtocol.Application.Services.Pdf;
using UltrasoundProtocol.Application.Services.Protocol;
using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class ProtocolsController : Controller
{
    private readonly IProtocolService _protocolService;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly IBreastProtocolService _breastProtocolService;
    private readonly IContentService _contentService;

    public ProtocolsController(IProtocolService protocolService, IPdfService pdfService,
        IExcelService excelService, IBreastProtocolService breastProtocolService, IContentService contentService)
    {
        _protocolService = protocolService;
        _pdfService = pdfService;
        _excelService = excelService;
        _breastProtocolService = breastProtocolService;
        _contentService = contentService;
    }

    public async Task<IActionResult> Index(string? search, string? status)
    {
        var protocols = await _protocolService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
            protocols = protocols.Where(p =>
                p.PatientName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.BodyPart.Contains(search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProtocolStatus>(status, out var st))
            protocols = protocols.Where(p => p.Status == st);

        ViewBag.Search = search;
        ViewBag.Status = status;
        return View(protocols);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var services = await _contentService.GetActiveServicesAsync();
        return View(services.OrderBy(s => s.SortOrder).ThenBy(s => s.Name));
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var protocol = await _protocolService.GetByIdAsync(id);
        if (protocol is null) return NotFound();

        var breastProtocol = IsBreastProtocol(protocol.BodyPart)
            ? await _breastProtocolService.GetPdfDataByExamIdAsync(id)
            : null;

        var pdf = breastProtocol is null
            ? _pdfService.GenerateProtocolPdf(protocol)
            : _pdfService.GenerateBreastProtocolPdf(protocol, breastProtocol);

        return File(pdf, "application/pdf", $"UZI_Protokol_{protocol.BodyPart}_{protocol.ExamDate:yyyyMMdd}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var protocols = await _protocolService.GetAllAsync();
        var excel = _excelService.ExportProtocols(protocols);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Protokollar.xlsx");
    }

    private static bool IsBreastProtocol(string bodyPart) =>
        bodyPart.Contains("Sut bez", StringComparison.OrdinalIgnoreCase);
}
