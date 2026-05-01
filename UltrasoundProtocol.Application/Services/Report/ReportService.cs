using AutoMapper;
using UltrasoundProtocol.Application.DTOs.Report;
using UltrasoundProtocol.Domain.Interfaces;
using ReportEntity = UltrasoundProtocol.Domain.Entities.Report;

namespace UltrasoundProtocol.Application.Services.Report;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReportDto?> GetByProtocolIdAsync(Guid protocolId)
    {
        var reports = await _unitOfWork.Reports.FindAsync(r => r.ProtocolId == protocolId);
        var report = reports.FirstOrDefault();
        return report is null ? null : _mapper.Map<ReportDto>(report);
    }

    public async Task<ReportDto> CreateAsync(ReportCreateDto dto, string generatedBy)
    {
        var report = _mapper.Map<ReportEntity>(dto);
        report.GeneratedBy = generatedBy;
        report.GeneratedAt = DateTime.UtcNow;

        await _unitOfWork.Reports.AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReportDto>(report);
    }
}
