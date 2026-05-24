using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.DTOs.Protocol;

public class ProtocolDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorUsername { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public ProtocolStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
