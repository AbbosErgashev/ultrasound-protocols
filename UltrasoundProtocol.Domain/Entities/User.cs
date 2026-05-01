using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;

    public ICollection<UltrasoundExam> UltrasoundProtocols { get; set; } = [];
    public ICollection<Diagnosis> Diagnoses { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
