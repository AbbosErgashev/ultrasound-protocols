namespace UltrasoundProtocol.Application.DTOs.Report;

public class ReportDto
{
    public Guid Id { get; set; }
    public Guid ProtocolId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
