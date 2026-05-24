using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Content;
using UltrasoundProtocol.Application.Services.Content;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "ContentManager,ChiefDoctor")]
public class ManageController : Controller
{
    private readonly IContentService _contentService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ManageController> _logger;

    public ManageController(IContentService contentService, IWebHostEnvironment env, ILogger<ManageController> logger)
    {
        _contentService = contentService;
        _env = env;
        _logger = logger;
    }

    // ===== NEWS =====

    public async Task<IActionResult> News()
    {
        var news = await _contentService.GetAllNewsAsync();
        return View(news);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewsCreate(NewsCreateDto dto, IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            if (!IsImageFile(imageFile))
            {
                TempData["Error"] = "Faqat rasm faylini yuklash mumkin";
                return RedirectToAction("News");
            }

            dto.ImageUrl = await SaveFileAsync(imageFile, "news/images");
        }

        dto.VideoUrl = null;

        await _contentService.CreateNewsAsync(dto, User.Identity?.Name ?? "admin");
        TempData["Success"] = "Yangilik qo'shildi";
        return RedirectToAction("News");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewsEdit(Guid id, NewsCreateDto dto, IFormFile? imageFile)
    {
        var current = await _contentService.GetNewsByIdAsync(id);
        if (current is null)
        {
            TempData["Error"] = "Yangilik topilmadi";
            return RedirectToAction("News");
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            if (!IsImageFile(imageFile))
            {
                TempData["Error"] = "Faqat rasm faylini yuklash mumkin";
                return RedirectToAction("News");
            }

            dto.ImageUrl = await SaveFileAsync(imageFile, "news/images");
        }
        else
        {
            dto.ImageUrl = current.ImageUrl;
        }

        dto.VideoUrl = null;

        await _contentService.UpdateNewsAsync(id, dto);
        TempData["Success"] = "Yangilik yangilandi";
        return RedirectToAction("News");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewsToggle(Guid id)
    {
        await _contentService.TogglePublishNewsAsync(id);
        return RedirectToAction("News");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewsDelete(Guid id)
    {
        await _contentService.DeleteNewsAsync(id);
        TempData["Success"] = "Yangilik o'chirildi";
        return RedirectToAction("News");
    }

    // ===== SERVICES =====

    public async Task<IActionResult> Services()
    {
        var services = await _contentService.GetAllServicesAsync();
        return View(services);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceCreate(ServiceCreateDto dto)
    {
        await _contentService.CreateServiceAsync(dto);
        TempData["Success"] = "Xizmat qo'shildi";
        return RedirectToAction("Services");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceEdit(Guid id, ServiceCreateDto dto)
    {
        await _contentService.UpdateServiceAsync(id, dto);
        TempData["Success"] = "Xizmat yangilandi";
        return RedirectToAction("Services");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceToggle(Guid id)
    {
        await _contentService.ToggleServiceAsync(id);
        return RedirectToAction("Services");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceDelete(Guid id)
    {
        await _contentService.DeleteServiceAsync(id);
        TempData["Success"] = "Xizmat o'chirildi";
        return RedirectToAction("Services");
    }

    // ===== DOCTORS =====

    public async Task<IActionResult> Doctors()
    {
        var doctors = await _contentService.GetAllDoctorsAsync();
        return View(doctors);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoctorCreate(DoctorProfileCreateDto dto)
    {
        await _contentService.CreateDoctorAsync(dto);
        TempData["Success"] = "Shifokor qo'shildi";
        return RedirectToAction("Doctors");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoctorEdit(Guid id, DoctorProfileCreateDto dto)
    {
        await _contentService.UpdateDoctorAsync(id, dto);
        TempData["Success"] = "Shifokor yangilandi";
        return RedirectToAction("Doctors");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoctorToggle(Guid id)
    {
        await _contentService.ToggleDoctorAsync(id);
        return RedirectToAction("Doctors");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoctorDelete(Guid id)
    {
        await _contentService.DeleteDoctorAsync(id);
        TempData["Success"] = "Shifokor o'chirildi";
        return RedirectToAction("Doctors");
    }

    // ===== HELPER =====

    private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(uploadsPath);
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsPath, fileName);
        using var stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream);
        return $"/uploads/{subFolder}/{fileName}";
    }

    private static bool IsImageFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        return file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) &&
               allowedExtensions.Contains(extension);
    }
}
