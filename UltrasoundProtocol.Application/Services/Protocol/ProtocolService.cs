using AutoMapper;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Domain.Entities;
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
        await AttachPatientsAsync(protocols);
        var result = _mapper.Map<IEnumerable<ProtocolDto>>(protocols);
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
        return _mapper.Map<ProtocolDto>(protocol);
    }

    public async Task<IEnumerable<ProtocolDto>> GetByPatientIdAsync(Guid patientId)
    {
        _logger.LogDebug("Bemor protokollari so'raldi: {PatientId}", patientId);
        var protocols = await _unitOfWork.UltrasoundExams
            .FindAsync(p => p.PatientId == patientId);
        await AttachPatientsAsync(protocols);
        return _mapper.Map<IEnumerable<ProtocolDto>>(protocols);
    }

    public async Task<ProtocolDto> CreateAsync(ProtocolCreateDto dto, string doctorUsername)
    {
        _logger.LogInformation("Yangi protokol yaratilmoqda: Organ={BodyPart}, Bemor={PatientId}, Shifokor={Doctor}",
            dto.BodyPart, dto.PatientId, doctorUsername);

        var exam = _mapper.Map<UltrasoundExam>(dto);
        exam.DoctorUsername = doctorUsername;

        await _unitOfWork.UltrasoundExams.AddAsync(exam);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Protokol yaratildi: {ProtocolId}, Organ: {BodyPart}", exam.Id, exam.BodyPart);
        return _mapper.Map<ProtocolDto>(exam);
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

    public async Task<bool> SaveAIAnalysisAsync(Guid protocolId, string aiResult)
    {
        _logger.LogInformation("AI tahlil natijasi saqlanmoqda: {ProtocolId}", protocolId);
        var exam = await _unitOfWork.UltrasoundExams.GetByIdAsync(protocolId);
        if (exam is null)
        {
            _logger.LogWarning("AI saqlash uchun protokol topilmadi: {ProtocolId}", protocolId);
            return false;
        }

        exam.AIAnalysisResult = aiResult;
        exam.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.UltrasoundExams.Update(exam);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("AI tahlil saqlandi: {ProtocolId}", protocolId);
        return true;
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
