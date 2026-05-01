namespace UltrasoundProtocol.Domain.Entities;

public class ServiceItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "🩺";
    public decimal? Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
