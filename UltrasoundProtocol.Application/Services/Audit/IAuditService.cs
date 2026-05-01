using UltrasoundProtocol.Domain.Entities;

namespace UltrasoundProtocol.Application.Services.Audit;

public interface IAuditService
{
    Task LogAsync(string username, string action, string entityName, string entityId,
        string? oldValues = null, string? newValues = null, string? ipAddress = null);
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetByUsernameAsync(string username);
}
