using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Domain.Entities;

public class UltrasoundExam : BaseEntity
{
    public Guid PatientId { get; set; }
    public string DoctorUsername { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public ProtocolStatus Status { get; set; } = ProtocolStatus.Active;

    public User Patient { get; set; } = null!;
    public ICollection<Diagnosis> Diagnoses { get; set; } = [];
    public Report? Report { get; set; }
}
