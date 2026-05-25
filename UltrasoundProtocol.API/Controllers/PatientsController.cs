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

    public async Task<IActionResult> Index(string? search, string? gender, string? status)
    {
        var patients = await _patientService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
            patients = patients.Where(p =>
                p.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (p.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        if (!string.IsNullOrWhiteSpace(gender))
            patients = patients.Where(p => p.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                patients = patients.Where(p => p.IsActive);
            else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                patients = patients.Where(p => !p.IsActive);
        }

        ViewBag.Search = search;
        ViewBag.Gender = gender;
        ViewBag.Status = status;

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
    public async Task<IActionResult> ChangeCredentials(
        Guid id,
        string newUsername,
        string newPassword,
        string? search,
        string? gender,
        string? status)
    {
        await _patientService.ChangeCredentialsAsync(id, newUsername, newPassword);
        TempData["Success"] = "Kirish ma'lumotlari yangilandi";
        return RedirectToAction("Index", new { search, gender, status });
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var patients = await _patientService.GetAllAsync();
        var excel = _excelService.ExportPatients(patients);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bemorlar.xlsx");
    }
}
