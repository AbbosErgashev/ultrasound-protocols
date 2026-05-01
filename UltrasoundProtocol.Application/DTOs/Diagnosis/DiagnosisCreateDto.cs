namespace UltrasoundProtocol.Application.DTOs.Diagnosis;

public class DiagnosisCreateDto
{
    public Guid PatientId { get; set; }
    public Guid ProtocolId { get; set; }
    public string IcdCode { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
