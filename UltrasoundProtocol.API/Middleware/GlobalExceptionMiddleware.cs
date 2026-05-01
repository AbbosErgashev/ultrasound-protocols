using System.Diagnostics;
using System.Net;

namespace UltrasoundProtocol.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
            sw.Stop();

            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {StatusCode} | {Method} {Path} | User={User} | IP={IP} | {ElapsedMs}ms",
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    context.User.Identity?.Name ?? "anonymous",
                    context.Connection.RemoteIpAddress,
                    sw.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "XATOLIK | {Method} {Path} | User={User} | IP={IP} | {ElapsedMs}ms | {ExceptionType}: {Message}",
                context.Request.Method,
                context.Request.Path,
                context.User.Identity?.Name ?? "anonymous",
                context.Connection.RemoteIpAddress,
                sw.ElapsedMilliseconds,
                ex.GetType().Name,
                ex.Message);

            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
