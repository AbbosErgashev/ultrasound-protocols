using UltrasoundProtocol.Application.DTOs.Appointment;

namespace UltrasoundProtocol.Application.Services.Appointment;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<AppointmentDto>> GetTodayAsync();
    Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto);
    Task<bool> ConfirmAsync(Guid id);
    Task<bool> CancelAsync(Guid id);
}
