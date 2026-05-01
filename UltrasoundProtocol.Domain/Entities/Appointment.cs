namespace UltrasoundProtocol.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public string DoctorUsername { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsCancelled { get; set; }

    public User Patient { get; set; } = null!;
}
