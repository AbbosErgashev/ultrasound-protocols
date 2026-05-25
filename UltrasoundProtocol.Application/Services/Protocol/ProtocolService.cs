using AutoMapper;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Enums;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Protocol;

public class ProtocolService : IProtocolService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProtocolService> _logger;

    public ProtocolService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProtocolService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProtocolDto>> GetAllAsync()
    {
        _logger.LogDebug("Barcha protokollar so'raldi");
        var protocols = await _unitOfWork.UltrasoundExams.GetAllAsync();
        var orderedProtocols = protocols.OrderByDescending(p => p.CreatedAt).ToList();
        await AttachPatientsAsync(orderedProtocols);
        var result = _mapper.Map<List<ProtocolDto>>(orderedProtocols);
        await AttachDoctorNamesAsync(result);
        _logger.LogInformation("Protokollar ro'yxati: {Count} ta", result.Count());
        return result;
    }

    public async Task<ProtocolDto?> GetByIdAsync(Guid id)
    {
        var protocol = await _unitOfWork.UltrasoundExams.GetByIdAsync(id);
        if (protocol is null)
        {
            _logger.LogWarning("Protokol topilmadi: {ProtocolId}", id);
            return null;
        }

        await AttachPatientsAsync([protocol]);
        var result = _mapper.Map<ProtocolDto>(protocol);
        await AttachDoctorNamesAsync([result]);
        return result;
    }

    public async Task<IEnumerable<ProtocolDto>> GetByPatientIdAsync(Guid patientId)
    {
        _logger.LogDebug("Bemor protokollari so'raldi: {PatientId}", patientId);
        var protocols = await _unitOfWork.UltrasoundExams
            .FindAsync(p => p.PatientId == patientId);
        var orderedProtocols = protocols.OrderByDescending(p => p.CreatedAt).ToList();
        await AttachPatientsAsync(orderedProtocols);
        var result = _mapper.Map<List<ProtocolDto>>(orderedProtocols);
        await AttachDoctorNamesAsync(result);
        return result;
    }

    public async Task<ProtocolDto?> UpdateAsync(Guid id, ProtocolUpdateDto dto)
    {
        _logger.LogInformation("Protokol yangilanmoqda: {ProtocolId}", id);
        var exam = await _unitOfWork.UltrasoundExams.GetByIdAsync(id);
        if (exam is null)
        {
            _logger.LogWarning("Yangilanish uchun protokol topilmadi: {ProtocolId}", id);
            return null;
        }

        exam.BodyPart = dto.BodyPart;
        exam.Findings = dto.Findings;
        exam.Conclusion = dto.Conclusion;
        exam.ImagePath = dto.ImagePath;
        exam.Status = dto.Status;
        exam.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UltrasoundExams.Update(exam);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Protokol yangilandi: {ProtocolId}, Holat: {Status}", id, dto.Status);
        return _mapper.Map<ProtocolDto>(exam);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, ProtocolStatus status)
    {
        _logger.LogInformation("Protokol holati yangilanmoqda: {ProtocolId}, Holat: {Status}", id, status);
        var exam = await _unitOfWork.UltrasoundExams.GetByIdAsync(id);
        if (exam is null)
        {
            _logger.LogWarning("Holatini yangilash uchun protokol topilmadi: {ProtocolId}", id);
            return false;
        }

        exam.Status = status;
        exam.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UltrasoundExams.Update(exam);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Protokol holati yangilandi: {ProtocolId}, Holat: {Status}", id, status);
        return true;
    }

    private async Task AttachDoctorNamesAsync(IEnumerable<ProtocolDto> protocols)
    {
        var protocolList = protocols.ToList();
        if (protocolList.Count == 0)
            return;

        var protocolIds = protocolList.Select(p => p.Id).ToHashSet();
        var breastProtocols = await _unitOfWork.BreastUltrasoundProtocols
            .FindAsync(p => protocolIds.Contains(p.UltrasoundExamId));

        var doctorNames = breastProtocols
            .Where(p => !string.IsNullOrWhiteSpace(p.DoctorName))
            .GroupBy(p => p.UltrasoundExamId)
            .ToDictionary(g => g.Key, g => g.First().DoctorName!.Trim());

        foreach (var protocol in protocolList)
        {
            if (doctorNames.TryGetValue(protocol.Id, out var doctorName))
                protocol.DoctorName = doctorName;
        }
    }

    private async Task AttachPatientsAsync(IEnumerable<UltrasoundExam> protocols)
    {
        var protocolList = protocols.ToList();
        if (protocolList.Count == 0)
            return;

        var patients = (await _unitOfWork.Users.GetAllAsync())
            .ToDictionary(patient => patient.Id);

        foreach (var protocol in protocolList)
        {
            if (protocol.Patient is null && patients.TryGetValue(protocol.PatientId, out var patient))
                protocol.Patient = patient;
        }
    }
}
