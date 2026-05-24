using UltrasoundProtocol.Application.DTOs.BreastProtocol;

namespace UltrasoundProtocol.Application.Services.BreastProtocol;

public interface IBreastProtocolService
{
    Task<Guid> CreateAsync(BreastProtocolCreateDto dto, string doctorUsername);
    Task<BreastProtocolPdfDto?> GetPdfDataByExamIdAsync(Guid ultrasoundExamId);
}
