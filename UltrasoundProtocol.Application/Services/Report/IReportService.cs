using UltrasoundProtocol.Application.DTOs.Report;

namespace UltrasoundProtocol.Application.Services.Report;

public interface IReportService
{
    Task<ReportDto?> GetByProtocolIdAsync(Guid protocolId);
    Task<ReportDto> CreateAsync(ReportCreateDto dto, string generatedBy);
}
