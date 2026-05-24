using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Excel;
using UltrasoundProtocol.Application.Services.Patient;
using UltrasoundProtocol.Application.Services.Pdf;
using UltrasoundProtocol.Application.Services.Protocol;
using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class ProtocolsController : Controller
{
    private readonly IProtocolService _protocolService;
    private readonly IPatientService _patientService;
    private readonly IPdfService _pdfService;
    private readonly IExcelService _excelService;
    private readonly IBreastProtocolService _breastProtocolService;

    public ProtocolsController(IProtocolService protocolService, IPatientService patientService,
        IPdfService pdfService, IExcelService excelService, IBreastProtocolService breastProtocolService)
    {
        _protocolService = protocolService;
        _patientService = patientService;
        _pdfService = pdfService;
        _excelService = excelService;
        _breastProtocolService = breastProtocolService;
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
        ViewBag.Patients = await _patientService.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProtocolCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Patients = await _patientService.GetAllAsync();
            return View(dto);
        }

        var doctorUsername = User.Identity?.Name ?? "";
        await _protocolService.CreateAsync(dto, doctorUsername);
        TempData["Success"] = "Protokol muvaffaqiyatli yaratildi";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var protocol = await _protocolService.GetByIdAsync(id);
        if (protocol is null) return NotFound();

        var breastProtocol = protocol.BodyPart.Equals("Sut bezlari", StringComparison.OrdinalIgnoreCase)
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
}
