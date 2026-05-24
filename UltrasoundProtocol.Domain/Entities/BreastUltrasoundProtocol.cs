namespace UltrasoundProtocol.Domain.Entities;

public class BreastUltrasoundProtocol : BaseEntity
{
    public Guid UltrasoundExamId { get; set; }
    public Guid PatientId { get; set; }
    public string DoctorUsername { get; set; } = string.Empty;
    public string? DoctorName { get; set; }
    public string? MedicalInstitutionName { get; set; }
    public string? MedicalInstitutionAddress { get; set; }
    public string? ProtocolNumber { get; set; }
    public DateTime ExamDate { get; set; }
    public string? UltrasoundMachine { get; set; }
    public string? Probe { get; set; }
    public string? UltrasoundExamNumber { get; set; }
    public decimal? PatientWeightKg { get; set; }
    public decimal? PatientHeightCm { get; set; }
    public string? Symmetry { get; set; }
    public string RightBreastJson { get; set; } = "{}";
    public string LeftBreastJson { get; set; } = "{}";
    public string LesionJson { get; set; } = "{}";
    public string CystsJson { get; set; } = "{}";
    public string RegionalLymphNodesJson { get; set; } = "{}";
    public string? AdditionalInfo { get; set; }
    public string Findings { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string? Birads { get; set; }
    public string? Recommendations { get; set; }

    public UltrasoundExam UltrasoundExam { get; set; } = null!;
    public User Patient { get; set; } = null!;
}
