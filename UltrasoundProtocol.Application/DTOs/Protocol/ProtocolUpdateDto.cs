using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.DTOs.Protocol;

public class ProtocolUpdateDto
{
    public string BodyPart { get; set; } = string.Empty;
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public ProtocolStatus Status { get; set; }
}
