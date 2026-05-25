using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.Services.Protocol;

public interface IProtocolService
{
    Task<IEnumerable<ProtocolDto>> GetAllAsync();
    Task<ProtocolDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProtocolDto>> GetByPatientIdAsync(Guid patientId);
    Task<ProtocolDto?> UpdateAsync(Guid id, ProtocolUpdateDto dto);
    Task<bool> UpdateStatusAsync(Guid id, ProtocolStatus status);
}
