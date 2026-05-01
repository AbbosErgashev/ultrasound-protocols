using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Appointment;
using UltrasoundProtocol.Application.Services.Appointment;
using UltrasoundProtocol.Application.Services.Patient;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IPatientService _patientService;

    public AppointmentsController(IAppointmentService appointmentService, IPatientService patientService)
    {
        _appointmentService = appointmentService;
        _patientService = patientService;
    }

    public async Task<IActionResult> Index()
    {
        var appointments = await _appointmentService.GetAllAsync();
        ViewBag.Patients = await _patientService.GetAllAsync();
        ViewBag.DoctorUsername = User.Identity?.Name ?? "";
        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Ma'lumotlar to'g'ri kiritilmagan";
            return RedirectToAction("Index");
        }
        await _appointmentService.CreateAsync(dto);
        TempData["Success"] = "Randevu muvaffaqiyatli qo'shildi";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _appointmentService.ConfirmAsync(id);
        TempData["Success"] = "Randevu tasdiqlandi";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _appointmentService.CancelAsync(id);
        TempData["Success"] = "Randevu bekor qilindi";
        return RedirectToAction("Index");
    }
}
