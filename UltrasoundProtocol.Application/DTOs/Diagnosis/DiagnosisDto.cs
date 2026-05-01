namespace UltrasoundProtocol.Application.DTOs.Diagnosis;

public class DiagnosisDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid ProtocolId { get; set; }
    public string IcdCode { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DoctorUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
