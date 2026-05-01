using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UltrasoundProtocol.Application.Settings;

namespace UltrasoundProtocol.Application.Services.AI;

public class AIAnalysisService : IAIAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly AISettings _settings;
    private readonly ILogger<AIAnalysisService> _logger;
    private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    private const string SystemPrompt = """
        Siz tajribali tibbiyot ultratovush (UZI) diagnostika mutaxassisisiz.
        Sizning vazifangiz — berilgan UZI tasviri yoki topilmalarni tahlil qilish.

        Javobingiz quyidagi tartibda bo'lsin:

        ## 🔍 Tasvir tahlili
        Tasvirda ko'rinayotgan organ va tuzilmalarni batafsil tavsiflang.

        ## ⚠️ Topilmalar
        Normal va anomal topilmalarni sanab o'ting.

        ## 🩺 Ehtimoliy tashxislar
        Mumkin bo'lgan tashxislarni ehtimollik darajasi bilan keltiring.

        ## 💊 Tavsiyalar
        Qo'shimcha tekshiruvlar yoki davolash bo'yicha tavsiyalar.

        ## ⚡ Xulosa
        Qisqacha umumiy xulosa.

        Diqqat: Bu AI tahlili bo'lib, yakuniy tashxis faqat shifokor tomonidan qo'yiladi.
        Javobni o'zbek tilida bering.
        """;

    public AIAnalysisService(HttpClient httpClient, IOptions<AISettings> settings, ILogger<AIAnalysisService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<AIAnalysisResult> AnalyzeImageAsync(
        byte[] imageData, string bodyPart, string? doctorFindings = null)
    {
        _logger.LogInformation("AI tasvir tahlili boshlandi: Organ={BodyPart}, RasmHajmi={ImageSize}KB",
            bodyPart, imageData.Length / 1024);

        if (!ValidateApiKey())
        {
            _logger.LogError("Anthropic API kaliti sozlanmagan");
            return ApiKeyMissingResult();
        }

        try
        {
            var base64Image = Convert.ToBase64String(imageData);
            var mediaType = DetectMediaType(imageData);
            var userPrompt = $"Ushbu UZI tasvirini tahlil qiling.\nTekshirilgan organ: {bodyPart}";
            if (!string.IsNullOrEmpty(doctorFindings))
                userPrompt += $"\nShifokor topilmalari: {doctorFindings}";

            var requestBody = new
            {
                system_instruction = new { parts = new[] { new { text = SystemPrompt } } },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { inline_data = new { mime_type = mediaType, data = base64Image } },
                            new { text = userPrompt }
                        }
                    }
                },
                generationConfig = new { maxOutputTokens = 2000 }
            };

            return await SendRequestAsync(requestBody);
        }
        catch (Exception ex)
        {
            return new AIAnalysisResult
            {
                Success = false,
                ErrorMessage = $"AI tahlilida xatolik: {ex.Message}"
            };
        }
    }

    public async Task<AIAnalysisResult> AnalyzeTextAsync(string findings, string bodyPart)
    {
        _logger.LogInformation("AI matnli tahlil boshlandi: Organ={BodyPart}", bodyPart);

        if (!ValidateApiKey())
        {
            _logger.LogError("Anthropic API kaliti sozlanmagan");
            return ApiKeyMissingResult();
        }

        try
        {
            var userPrompt = $"""
                Quyidagi UZI tekshiruvi topilmalarini tahlil qiling va professional xulosangizni bering.

                Tekshirilgan organ: {bodyPart}
                Shifokor topilmalari: {findings}
                """;

            var requestBody = new
            {
                system_instruction = new { parts = new[] { new { text = SystemPrompt } } },
                contents = new[]
                {
                    new { role = "user", parts = new[] { new { text = userPrompt } } }
                },
                generationConfig = new { maxOutputTokens = 2000 }
            };

            return await SendRequestAsync(requestBody);
        }
        catch (Exception ex)
        {
            return new AIAnalysisResult
            {
                Success = false,
                ErrorMessage = $"AI tahlilida xatolik: {ex.Message}"
            };
        }
    }

    private async Task<AIAnalysisResult> SendRequestAsync(object requestBody)
    {
        var sw = Stopwatch.StartNew();
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{GeminiBaseUrl}/{_settings.ModelName}:generateContent?key={_settings.ApiKey}";
        _logger.LogDebug("Gemini API ga so'rov yuborilmoqda: Model={Model}", _settings.ModelName);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();
        sw.Stop();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Gemini API xatosi: StatusCode={StatusCode}, Vaqt={ElapsedMs}ms, Javob={Response}",
                response.StatusCode, sw.ElapsedMilliseconds, responseJson[..Math.Min(500, responseJson.Length)]);
            return new AIAnalysisResult
            {
                Success = false,
                ErrorMessage = ParseApiErrorMessage(response.StatusCode.ToString(), responseJson)
            };
        }

        using var doc = JsonDocument.Parse(responseJson);
        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        _logger.LogInformation("Gemini API muvaffaqiyatli: Vaqt={ElapsedMs}ms, JavobUzunligi={Length}",
            sw.ElapsedMilliseconds, text?.Length ?? 0);

        return new AIAnalysisResult
        {
            Success = true,
            Analysis = text ?? "Natija bo'sh qaytdi."
        };
    }

    private string ParseApiErrorMessage(string statusCode, string responseJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var error))
            {
                var message = error.TryGetProperty("message", out var msg) ? msg.GetString() ?? "" : "";
                var type = error.TryGetProperty("type", out var t) ? t.GetString() ?? "" : "";

                if (message.Contains("API key not valid", StringComparison.OrdinalIgnoreCase) ||
                    type == "UNAUTHENTICATED" || statusCode == "Unauthorized")
                    return "Gemini API kaliti noto'g'ri. Iltimos, https://aistudio.google.com sahifasidan yangi kalit olib appsettings.json → AISettings:ApiKey ga qo'ying.";

                if (type == "RESOURCE_EXHAUSTED" || statusCode == "TooManyRequests" ||
                    message.Contains("quota", StringComparison.OrdinalIgnoreCase))
                    return "Gemini API bepul limitiga yetildi (kuniga 1500 so'rov). Ertaga qayta urinib ko'ring yoki boshqa API kaliti oling.";

                if (type == "NOT_FOUND" || statusCode == "NotFound" ||
                    message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return $"Gemini model topilmadi: '{_settings.ModelName}'. appsettings.json → AISettings:ModelName ni tekshiring (masalan: gemini-1.5-flash).";

                if (type == "PERMISSION_DENIED" || statusCode == "Forbidden")
                    return "Gemini API ga kirish rad etildi. API kalitingiz ushbu xizmatga ruxsat berilganligini tekshiring.";

                if (type == "UNAVAILABLE" || statusCode == "ServiceUnavailable")
                    return "Gemini API hozirda band. Bir oz kutib qayta urinib ko'ring.";

                if (!string.IsNullOrEmpty(message))
                    return $"Claude API xatosi: {message}";
            }
        }
        catch
        {
            // JSON parse xatosi — quyida umumiy xabar qaytariladi
        }

        return $"Gemini API xatosi ({statusCode}). Iltimos, qayta urinib ko'ring.";
    }

    private static string DetectMediaType(byte[] data)
    {
        if (data.Length >= 4 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
            return "image/png";
        if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
            return "image/jpeg";
        if (data.Length >= 4 && data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x46)
            return "image/webp";
        if (data.Length >= 6 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46)
            return "image/gif";
        return "image/jpeg";
    }

    private bool ValidateApiKey() =>
        !string.IsNullOrEmpty(_settings.ApiKey) &&
        !_settings.ApiKey.StartsWith("your-") &&
        _settings.ApiKey.Length > 10;

    private static AIAnalysisResult ApiKeyMissingResult() => new()
    {
        Success = false,
        ErrorMessage = "Gemini API kaliti sozlanmagan. https://aistudio.google.com sahifasidan bepul kalit olib appsettings.json → AISettings:ApiKey ga qo'ying."
    };
}
