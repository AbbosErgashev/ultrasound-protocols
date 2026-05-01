using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.DTOs.Auth;
using UltrasoundProtocol.Application.Services.Auth;

namespace UltrasoundProtocol.API.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? error = null, string? info = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        ViewBag.Error = error;
        ViewBag.Info = info;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        _logger.LogInformation("Login urinishi: {Username} | IP: {ClientIP}", request.Username, clientIp);

        var result = await _authService.LoginAsync(request);
        if (result is null)
        {
            _logger.LogWarning("Login muvaffaqiyatsiz: {Username} | IP: {ClientIP}", request.Username, clientIp);
            ViewBag.Error = "Login yoki parol noto'g'ri";
            ViewBag.Info = "Agar siz ushbu muassasa bemori bo'lmasangiz, tizimga kirish imkoniyatingiz yo'q. Tekshiruvdan o'tish uchun markaz qabulxonasiga murojaat qiling.";
            return View(request);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.Username),
            new(ClaimTypes.GivenName, result.FullName),
            new(ClaimTypes.Role, result.Role),
            new("UserId", result.UserId?.ToString() ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        _logger.LogInformation("Login muvaffaqiyatli: {FullName} | Rol: {Role} | IP: {ClientIP}",
            result.FullName, result.Role, clientIp);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var userName = User.Identity?.Name ?? "unknown";
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Logout: {UserName}", userName);
        return RedirectToAction("Login");
    }
}
