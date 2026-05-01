namespace UltrasoundProtocol.Application.DTOs.Report;

public class ReportCreateDto
{
    public Guid ProtocolId { get; set; }
    public string Content { get; set; } = string.Empty;
}
