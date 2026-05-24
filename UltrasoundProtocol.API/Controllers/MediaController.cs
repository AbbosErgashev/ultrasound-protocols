using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UltrasoundProtocol.API.Controllers;

[AllowAnonymous]
public class MediaController : Controller
{
    private const long MaxMediaBytes = 50 * 1024 * 1024;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IHttpClientFactory httpClientFactory, ILogger<MediaController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Proxy([FromQuery] string url, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest();
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(20);

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 AppleWebKit/537.36 Chrome/124.0 Safari/537.36");
        request.Headers.Referrer = new Uri($"{uri.Scheme}://{uri.Host}/");

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Media proxy failed: {Url}, Status: {StatusCode}", url, response.StatusCode);
            return StatusCode((int)response.StatusCode);
        }

        var contentLength = response.Content.Headers.ContentLength;
        if (contentLength is > MaxMediaBytes)
            return BadRequest();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) &&
            !contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
        var media = new MemoryStream();
        await source.CopyToAsync(media, cancellationToken);
        media.Position = 0;

        Response.Headers.CacheControl = "public,max-age=3600";
        return File(media, contentType);
    }
}
