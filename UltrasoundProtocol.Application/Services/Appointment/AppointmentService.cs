using AutoMapper;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.Appointment;
using UltrasoundProtocol.Domain.Interfaces;
using AppointmentEntity = UltrasoundProtocol.Domain.Entities.Appointment;

namespace UltrasoundProtocol.Application.Services.Appointment;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        _logger.LogDebug("Barcha randevular so'raldi");
        var items = await _unitOfWork.Appointments.GetAllAsync();
        var orderedItems = items.OrderByDescending(a => a.CreatedAt).ToList();
        await AttachPatientsAsync(orderedItems);
        return _mapper.Map<IEnumerable<AppointmentDto>>(orderedItems);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId)
    {
        var items = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId);
        var orderedItems = items.OrderByDescending(a => a.CreatedAt).ToList();
        await AttachPatientsAsync(orderedItems);
        return _mapper.Map<IEnumerable<AppointmentDto>>(orderedItems);
    }

    public async Task<IEnumerable<AppointmentDto>> GetTodayAsync()
    {
        var today = DateTime.Today;
        var items = await _unitOfWork.Appointments.FindAsync(
            a => a.AppointmentDate.Date == today && !a.IsCancelled);
        var orderedItems = items.OrderBy(a => a.AppointmentDate).ToList();
        await AttachPatientsAsync(orderedItems);
        return _mapper.Map<IEnumerable<AppointmentDto>>(orderedItems);
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto)
    {
        _logger.LogInformation("Yangi randevu: Bemor={PatientId}, Sana={Date}, Shifokor={Doctor}",
            dto.PatientId, dto.AppointmentDate, dto.DoctorUsername);
        var entity = _mapper.Map<AppointmentEntity>(dto);
        await _unitOfWork.Appointments.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        await AttachPatientsAsync([entity]);
        _logger.LogInformation("Randevu yaratildi: {AppointmentId}", entity.Id);
        return _mapper.Map<AppointmentDto>(entity);
    }

    public async Task<bool> ConfirmAsync(Guid id)
    {
        var entity = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (entity is null) return false;
        entity.IsConfirmed = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Appointments.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var entity = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (entity is null) return false;
        entity.IsCancelled = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Appointments.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task AttachPatientsAsync(IEnumerable<AppointmentEntity> appointments)
    {
        var appointmentList = appointments.ToList();
        if (appointmentList.Count == 0)
            return;

        var patientIds = appointmentList
            .Select(a => a.PatientId)
            .Distinct()
            .ToArray();

        var patients = (await _unitOfWork.Users.FindAsync(u => patientIds.Contains(u.Id)))
            .ToDictionary(patient => patient.Id);

        foreach (var appointment in appointmentList)
        {
            if (appointment.Patient is null && patients.TryGetValue(appointment.PatientId, out var patient))
                appointment.Patient = patient;
        }
    }
}
