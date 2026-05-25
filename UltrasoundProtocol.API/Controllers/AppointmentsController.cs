using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Appointment;
using UltrasoundProtocol.Application.Services.Appointment;
using UltrasoundProtocol.Application.Services.Content;
using UltrasoundProtocol.Application.Services.Patient;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IPatientService _patientService;
    private readonly IContentService _contentService;

    public AppointmentsController(
        IAppointmentService appointmentService,
        IPatientService patientService,
        IContentService contentService)
    {
        _appointmentService = appointmentService;
        _patientService = patientService;
        _contentService = contentService;
    }

    public async Task<IActionResult> Index()
    {
        var appointments = await _appointmentService.GetAllAsync();
        ViewBag.Patients = await _patientService.GetAllAsync();
        ViewBag.Doctors = (await _contentService.GetActiveDoctorsAsync())
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.FullName);
        return View(appointments);
    }

    [HttpGet]
    public async Task<IActionResult> Today()
    {
        var appointments = await _appointmentService.GetTodayAsync();
        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateDto dto)
    {
        var doctors = (await _contentService.GetActiveDoctorsAsync()).ToList();

        if (dto.PatientId == Guid.Empty)
            ModelState.AddModelError(nameof(dto.PatientId), "Bemor tanlanishi shart");

        var doctor = dto.DoctorProfileId == Guid.Empty
            ? null
            : doctors.FirstOrDefault(d => d.Id == dto.DoctorProfileId);

        if (dto.DoctorProfileId == Guid.Empty)
            ModelState.AddModelError(nameof(dto.DoctorProfileId), "Shifokor tanlanishi shart");
        else if (doctor is null)
            ModelState.AddModelError(nameof(dto.DoctorProfileId), "Tanlangan shifokor topilmadi");

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Ma'lumotlar to'g'ri kiritilmagan";
            return RedirectToAction("Index");
        }

        dto.DoctorUsername = doctor!.FullName.Trim();
        await _appointmentService.CreateAsync(dto);
        TempData["Success"] = "Randevu muvaffaqiyatli qo'shildi";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id, string? returnUrl)
    {
        await _appointmentService.ConfirmAsync(id);
        TempData["Success"] = "Randevu tasdiqlandi";
        return RedirectToLocalOrIndex(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? returnUrl)
    {
        await _appointmentService.CancelAsync(id);
        TempData["Success"] = "Randevu bekor qilindi";
        return RedirectToLocalOrIndex(returnUrl);
    }

    private IActionResult RedirectToLocalOrIndex(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }
}
