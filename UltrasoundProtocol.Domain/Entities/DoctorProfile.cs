namespace UltrasoundProtocol.Domain.Entities;

public class DoctorProfile : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public string? Phone { get; set; }
    public int ExperienceYears { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
