namespace UltrasoundProtocol.Application.DTOs.BreastProtocol;

public class BreastProtocolCreateDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorProfileId { get; set; }
    public string? MedicalInstitutionName { get; set; }
    public string? MedicalInstitutionAddress { get; set; }
    public string? ProtocolNumber { get; set; }
    public DateTime ExamDate { get; set; } = DateTime.Today;
    public string? UltrasoundMachine { get; set; }
    public string? Probe { get; set; }
    public string? UltrasoundExamNumber { get; set; }
    public decimal? PatientWeightKg { get; set; }
    public decimal? PatientHeightCm { get; set; }
    public string? Symmetry { get; set; }
    public BreastSideDto Right { get; set; } = new();
    public BreastSideDto Left { get; set; } = new();
    public BreastLesionDto Lesion { get; set; } = new();
    public BreastCystsDto Cysts { get; set; } = new();
    public RegionalLymphNodesDto RegionalLymphNodes { get; set; } = new();
    public string? AdditionalInfo { get; set; }
    public string? Conclusion { get; set; }
    public string? Birads { get; set; }
    public string? Recommendations { get; set; }
}

public class BreastSideDto
{
    public string? SkinLine { get; set; }
    public decimal? SkinThicknessMm { get; set; }
    public string? Structure { get; set; }
    public string? Nipple { get; set; }
    public string? RetroareolarZone { get; set; }
    public string? RetroareolarEchostructure { get; set; }
    public string? TissueDifferentiation { get; set; }
    public string? SubcutaneousFat { get; set; }
    public string? SubcutaneousFatEchostructure { get; set; }
    public decimal? FibroglandularComplexThicknessMm { get; set; }
    public string? TissueRatio { get; set; }
    public string? Echostructure { get; set; }
    public string? FocalChanges { get; set; }
    public string? Ducts { get; set; }
    public string? DuctWall { get; set; }
    public string? DuctContent { get; set; }
    public string? RetromammaryFat { get; set; }
    public string? IntramammaryLymphNodes { get; set; }
}

public class BreastLesionDto
{
    public bool Detected { get; set; }
    public string? Breast { get; set; }
    public string? Zone { get; set; }
    public string? SizeMm { get; set; }
    public string? Shape { get; set; }
    public string? Orientation { get; set; }
    public string? Contours { get; set; }
    public string? Structure { get; set; }
    public string? Echogenicity { get; set; }
    public string? DistalAcousticEffect { get; set; }
    public string? Calcifications { get; set; }
    public decimal? CalcificationSizeMm { get; set; }
    public string? AdditionalChanges { get; set; }
}

public class BreastCystsDto
{
    public string? UpperOuterRight { get; set; }
    public string? UpperOuterLeft { get; set; }
    public string? UpperInnerRight { get; set; }
    public string? UpperInnerLeft { get; set; }
    public string? LowerOuterRight { get; set; }
    public string? LowerOuterLeft { get; set; }
    public string? LowerInnerRight { get; set; }
    public string? LowerInnerLeft { get; set; }
    public string? OuterBorderRight { get; set; }
    public string? OuterBorderLeft { get; set; }
    public string? UpperBorderRight { get; set; }
    public string? UpperBorderLeft { get; set; }
    public string? InnerBorderRight { get; set; }
    public string? InnerBorderLeft { get; set; }
    public string? LowerBorderRight { get; set; }
    public string? LowerBorderLeft { get; set; }
}

public class RegionalLymphNodesDto
{
    public string? RightSizeMm { get; set; }
    public string? LeftSizeMm { get; set; }
    public string? Count { get; set; }
    public string? Structure { get; set; }
}

public class BreastProtocolPdfDto
{
    public Guid Id { get; set; }
    public Guid UltrasoundExamId { get; set; }
    public string? DoctorName { get; set; }
    public string? MedicalInstitutionName { get; set; }
    public string? MedicalInstitutionAddress { get; set; }
    public string? ProtocolNumber { get; set; }
    public string? UltrasoundMachine { get; set; }
    public string? Probe { get; set; }
    public string? UltrasoundExamNumber { get; set; }
    public decimal? PatientWeightKg { get; set; }
    public decimal? PatientHeightCm { get; set; }
    public string? Symmetry { get; set; }
    public BreastSideDto Right { get; set; } = new();
    public BreastSideDto Left { get; set; } = new();
    public BreastLesionDto Lesion { get; set; } = new();
    public BreastCystsDto Cysts { get; set; } = new();
    public RegionalLymphNodesDto RegionalLymphNodes { get; set; } = new();
    public string? AdditionalInfo { get; set; }
    public string Conclusion { get; set; } = string.Empty;
    public string? Birads { get; set; }
    public string? Recommendations { get; set; }
}
