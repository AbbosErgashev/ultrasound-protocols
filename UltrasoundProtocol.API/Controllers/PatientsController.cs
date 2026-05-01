using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Patient;
using UltrasoundProtocol.Application.Services.Excel;
using UltrasoundProtocol.Application.Services.Patient;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class PatientsController : Controller
{
    private readonly IPatientService _patientService;
    private readonly IExcelService _excelService;

    public PatientsController(IPatientService patientService, IExcelService excelService)
    {
        _patientService = patientService;
        _excelService = excelService;
    }

    public async Task<IActionResult> Index()
    {
        var patients = await _patientService.GetAllAsync();
        return View(patients);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PatientCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var doctorUsername = User.Identity?.Name ?? "";
        var patient = await _patientService.CreateAsync(dto, doctorUsername);

        TempData["PatientName"] = patient.FullName;
        TempData["PatientUsername"] = patient.Username;
        TempData["PatientPassword"] = dto.Password;
        TempData["PatientPhone"] = patient.PhoneNumber;

        return RedirectToAction("CreateSuccess");
    }

    public IActionResult CreateSuccess()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ChiefDoctor")]
    public async Task<IActionResult> ChangeCredentials(Guid id, string newUsername, string newPassword)
    {
        await _patientService.ChangeCredentialsAsync(id, newUsername, newPassword);
        TempData["Success"] = "Kirish ma'lumotlari yangilandi";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var patients = await _patientService.GetAllAsync();
        var excel = _excelService.ExportPatients(patients);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bemorlar.xlsx");
    }
}
