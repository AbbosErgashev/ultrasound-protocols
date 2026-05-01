namespace UltrasoundProtocol.Domain.Entities;

public class Diagnosis : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProtocolId { get; set; }
    public string IcdCode { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DoctorUsername { get; set; } = string.Empty;

    public User Patient { get; set; } = null!;
    public UltrasoundExam Protocol { get; set; } = null!;
}
