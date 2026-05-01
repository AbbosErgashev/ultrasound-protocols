namespace UltrasoundProtocol.Application.DTOs.Appointment;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorUsername { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
}
