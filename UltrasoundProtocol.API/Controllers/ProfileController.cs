using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UltrasoundProtocol.API.Controllers;

[Authorize]
public class ProfileController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Username = User.Identity?.Name;
        ViewBag.FullName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
        ViewBag.Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return View();
    }
}
