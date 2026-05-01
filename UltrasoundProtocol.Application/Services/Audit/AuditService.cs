using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Audit;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IUnitOfWork unitOfWork, ILogger<AuditService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogAsync(
        string username, string action, string entityName, string entityId,
        string? oldValues = null, string? newValues = null, string? ipAddress = null)
    {
        _logger.LogInformation("AUDIT: {Username} | {Action} | {Entity}:{EntityId} | IP:{IP}",
            username, action, entityName, entityId, ipAddress ?? "N/A");

        var log = new AuditLog
        {
            Username = username,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        await _unitOfWork.AuditLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _unitOfWork.AuditLogs.GetAllAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUsernameAsync(string username)
    {
        return await _unitOfWork.AuditLogs
            .FindAsync(a => a.Username == username);
    }
}
