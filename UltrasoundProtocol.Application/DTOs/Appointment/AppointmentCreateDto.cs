namespace UltrasoundProtocol.Application.DTOs.Appointment;

public class AppointmentCreateDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorProfileId { get; set; }
    public string DoctorUsername { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
}
