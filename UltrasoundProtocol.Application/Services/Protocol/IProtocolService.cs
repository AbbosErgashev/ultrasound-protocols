using UltrasoundProtocol.Application.DTOs.Protocol;

namespace UltrasoundProtocol.Application.Services.Protocol;

public interface IProtocolService
{
    Task<IEnumerable<ProtocolDto>> GetAllAsync();
    Task<ProtocolDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProtocolDto>> GetByPatientIdAsync(Guid patientId);
    Task<ProtocolDto> CreateAsync(ProtocolCreateDto dto, string doctorUsername);
    Task<ProtocolDto?> UpdateAsync(Guid id, ProtocolUpdateDto dto);
}
