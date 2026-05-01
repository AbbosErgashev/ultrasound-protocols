using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.Content;

namespace UltrasoundProtocol.API.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IContentService _contentService;

    public HomeController(IContentService contentService)
    {
        _contentService = contentService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.News = await _contentService.GetPublishedNewsAsync();
        ViewBag.Services = await _contentService.GetActiveServicesAsync();
        ViewBag.Doctors = await _contentService.GetActiveDoctorsAsync();
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
