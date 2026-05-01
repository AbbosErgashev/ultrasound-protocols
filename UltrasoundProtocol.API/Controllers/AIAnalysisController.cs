using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltrasoundProtocol.Application.Services.AI;
using UltrasoundProtocol.Application.Services.Protocol;

namespace UltrasoundProtocol.API.Controllers;

[Authorize(Roles = "Doctor,SeniorDoctor,ChiefDoctor")]
public class AIAnalysisController : Controller
{
    private readonly IAIAnalysisService _aiService;
    private readonly IProtocolService _protocolService;

    public AIAnalysisController(IAIAnalysisService aiService, IProtocolService protocolService)
    {
        _aiService = aiService;
        _protocolService = protocolService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeImage(IFormFile imageFile, string bodyPart, string? doctorFindings)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            ViewBag.Error = "Rasm tanlanmadi";
            return View("Index");
        }

        using var ms = new MemoryStream();
        await imageFile.CopyToAsync(ms);
        var imageData = ms.ToArray();

        var result = await _aiService.AnalyzeImageAsync(imageData, bodyPart, doctorFindings);

        ViewBag.Analysis = result.Analysis;
        ViewBag.Success = result.Success;
        ViewBag.Error = result.ErrorMessage;
        ViewBag.BodyPart = bodyPart;
        return View("Result");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeText(string findings, string bodyPart)
    {
        if (string.IsNullOrWhiteSpace(findings))
        {
            ViewBag.Error = "Topilmalar kiritilmadi";
            return View("Index");
        }

        var result = await _aiService.AnalyzeTextAsync(findings, bodyPart);

        ViewBag.Analysis = result.Analysis;
        ViewBag.Success = result.Success;
        ViewBag.Error = result.ErrorMessage;
        ViewBag.BodyPart = bodyPart;
        return View("Result");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveToProtocol(Guid protocolId, string analysisResult)
    {
        await _protocolService.SaveAIAnalysisAsync(protocolId, analysisResult);
        TempData["Success"] = "AI tahlil protokolga saqlandi";
        return RedirectToAction("Index", "Protocols");
    }
}
