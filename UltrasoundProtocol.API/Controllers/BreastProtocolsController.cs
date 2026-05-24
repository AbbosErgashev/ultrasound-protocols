using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.BreastProtocol;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Patient;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class BreastProtocolsController : Controller
{
    private readonly IBreastProtocolService _breastProtocolService;
    private readonly IPatientService _patientService;

    public BreastProtocolsController(
        IBreastProtocolService breastProtocolService,
        IPatientService patientService)
    {
        _breastProtocolService = breastProtocolService;
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await _patientService.GetAllAsync();
        return View(new BreastProtocolCreateDto
        {
            ExamDate = DateTime.Today,
            Symmetry = "Simmetrik",
            Right = CreateDefaultSide(),
            Left = CreateDefaultSide()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BreastProtocolCreateDto dto)
    {
        if (dto.PatientId == Guid.Empty)
            ModelState.AddModelError(nameof(dto.PatientId), "Bemor tanlanishi shart");

        if (!ModelState.IsValid)
        {
            ViewBag.Patients = await _patientService.GetAllAsync();
            return View(dto);
        }

        var doctorUsername = User.Identity?.Name ?? "";
        await _breastProtocolService.CreateAsync(dto, doctorUsername);
        TempData["Success"] = "Sut bezlari UTT protokoli muvaffaqiyatli yaratildi";
        return RedirectToAction("Index", "Protocols");
    }

    private static BreastSideDto CreateDefaultSide() => new()
    {
        SkinLine = "Tekis giperechogen chiziq",
        SkinThicknessMm = 3,
        Structure = "Bir xil",
        Nipple = "Yumaloq shaklda",
        RetroareolarZone = "Ko'rinadi",
        RetroareolarEchostructure = "Bir xil",
        TissueDifferentiation = "Yaxshi",
        SubcutaneousFat = "Sust ifodalangan",
        SubcutaneousFatEchostructure = "Bir xil",
        TissueRatio = "Bez va yog' to'qimasi teng miqdorda",
        FocalChanges = "Aniqlanmadi",
        Ducts = "Kengaymagan",
        DuctWall = "Bir xil",
        DuctContent = "Anexogen",
        RetromammaryFat = "Xususiyatsiz",
        IntramammaryLymphNodes = "Ko'rinmaydi"
    };
}
