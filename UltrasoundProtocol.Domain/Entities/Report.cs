namespace UltrasoundProtocol.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ProtocolId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string GeneratedBy { get; set; } = string.Empty;

    public UltrasoundExam Protocol { get; set; } = null!;
}
