namespace UltrasoundProtocol.Application.DTOs.Protocol;

public class ProtocolCreateDto
{
    public Guid PatientId { get; set; }
    public string BodyPart { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
