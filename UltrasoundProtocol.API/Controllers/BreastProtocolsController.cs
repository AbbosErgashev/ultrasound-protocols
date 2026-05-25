using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.BreastProtocol;
using UltrasoundProtocol.Application.DTOs.Content;
using UltrasoundProtocol.Application.Services.BreastProtocol;
using UltrasoundProtocol.Application.Services.Content;
using UltrasoundProtocol.Application.Services.Patient;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class BreastProtocolsController : Controller
{
    private const string MedicalInstitutionName = "MedUZI Diagnostika Markazi";

    private readonly IBreastProtocolService _breastProtocolService;
    private readonly IPatientService _patientService;
    private readonly IContentService _contentService;

    public BreastProtocolsController(
        IBreastProtocolService breastProtocolService,
        IPatientService patientService,
        IContentService contentService)
    {
        _breastProtocolService = breastProtocolService;
        _patientService = patientService;
        _contentService = contentService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new BreastProtocolCreateDto
        {
            ExamDate = DateTime.Today,
            MedicalInstitutionName = MedicalInstitutionName,
            MedicalInstitutionAddress = "Toshkent shahri, Chilonzor tumani, Bunyodkor shoh ko'chasi, 42-uy",
            Symmetry = "Simmetrik",
            Right = CreateDefaultSide(),
            Left = CreateDefaultSide()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BreastProtocolCreateDto dto, string? submitAction)
    {
        dto.MedicalInstitutionName = MedicalInstitutionName;

        var doctors = (await _contentService.GetActiveDoctorsAsync()).ToList();

        if (dto.PatientId == Guid.Empty)
            ModelState.AddModelError(nameof(dto.PatientId), "Bemor tanlanishi shart");

        if (dto.DoctorProfileId == Guid.Empty)
            ModelState.AddModelError(nameof(dto.DoctorProfileId), "Shifokor tanlanishi shart");
        else if (!doctors.Any(d => d.Id == dto.DoctorProfileId))
            ModelState.AddModelError(nameof(dto.DoctorProfileId), "Tanlangan shifokor topilmadi");

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(doctors);
            return View(dto);
        }

        var doctorUsername = User.Identity?.Name ?? "";
        var protocolId = await _breastProtocolService.CreateAsync(dto, doctorUsername);

        if (string.Equals(submitAction, "print", StringComparison.OrdinalIgnoreCase))
            return RedirectToAction("ExportPdf", "Protocols", new { id = protocolId });

        TempData["Success"] = "Sut bezining Ultratovush protokoli muvaffaqiyatli yaratildi";
        return RedirectToAction("Index", "Protocols");
    }

    private async Task LoadDropdownsAsync(IEnumerable<DoctorProfileDto>? doctors = null)
    {
        ViewBag.Patients = await _patientService.GetAllAsync();
        ViewBag.Doctors = (doctors ?? await _contentService.GetActiveDoctorsAsync())
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.FullName);
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
