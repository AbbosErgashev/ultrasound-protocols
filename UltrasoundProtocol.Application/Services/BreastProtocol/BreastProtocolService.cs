using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UltrasoundProtocol.Application.DTOs.BreastProtocol;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.BreastProtocol;

public class BreastProtocolService : IBreastProtocolService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BreastProtocolService> _logger;

    public BreastProtocolService(IUnitOfWork unitOfWork, ILogger<BreastProtocolService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(BreastProtocolCreateDto dto, string doctorUsername)
    {
        var findings = BuildFindings(dto);
        var conclusion = BuildConclusion(dto);
        var recommendations = string.IsNullOrWhiteSpace(dto.Recommendations)
            ? "Klinik ko'rsatmalar bo'lsa, tegishli mutaxassis konsultatsiyasi tavsiya etiladi."
            : dto.Recommendations.Trim();

        var exam = new UltrasoundExam
        {
            PatientId = dto.PatientId,
            DoctorUsername = doctorUsername,
            BodyPart = "Sut bezlari",
            ExamDate = dto.ExamDate == default ? DateTime.Today : dto.ExamDate,
            Findings = findings,
            Conclusion = conclusion,
        };

        await _unitOfWork.UltrasoundExams.AddAsync(exam);

        var breastProtocol = new BreastUltrasoundProtocol
        {
            UltrasoundExamId = exam.Id,
            PatientId = dto.PatientId,
            DoctorUsername = doctorUsername,
            DoctorName = Clean(dto.DoctorName),
            MedicalInstitutionName = Clean(dto.MedicalInstitutionName),
            MedicalInstitutionAddress = Clean(dto.MedicalInstitutionAddress),
            ProtocolNumber = Clean(dto.ProtocolNumber),
            ExamDate = exam.ExamDate,
            UltrasoundMachine = Clean(dto.UltrasoundMachine),
            Probe = Clean(dto.Probe),
            UltrasoundExamNumber = Clean(dto.UltrasoundExamNumber),
            PatientWeightKg = dto.PatientWeightKg,
            PatientHeightCm = dto.PatientHeightCm,
            Symmetry = Clean(dto.Symmetry),
            RightBreastJson = JsonSerializer.Serialize(dto.Right, JsonOptions),
            LeftBreastJson = JsonSerializer.Serialize(dto.Left, JsonOptions),
            LesionJson = JsonSerializer.Serialize(dto.Lesion, JsonOptions),
            CystsJson = JsonSerializer.Serialize(dto.Cysts, JsonOptions),
            RegionalLymphNodesJson = JsonSerializer.Serialize(dto.RegionalLymphNodes, JsonOptions),
            AdditionalInfo = Clean(dto.AdditionalInfo),
            Findings = findings,
            Conclusion = conclusion,
            Birads = Clean(dto.Birads),
            Recommendations = recommendations,
        };

        await _unitOfWork.BreastUltrasoundProtocols.AddAsync(breastProtocol);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Sut bezlari UTT protokoli yaratildi: {ProtocolId}", breastProtocol.Id);
        return exam.Id;
    }

    public async Task<BreastProtocolPdfDto?> GetPdfDataByExamIdAsync(Guid ultrasoundExamId)
    {
        var protocol = (await _unitOfWork.BreastUltrasoundProtocols
            .FindAsync(p => p.UltrasoundExamId == ultrasoundExamId))
            .FirstOrDefault();

        if (protocol is null)
            return null;

        return new BreastProtocolPdfDto
        {
            Id = protocol.Id,
            UltrasoundExamId = protocol.UltrasoundExamId,
            DoctorName = protocol.DoctorName,
            MedicalInstitutionName = protocol.MedicalInstitutionName,
            MedicalInstitutionAddress = protocol.MedicalInstitutionAddress,
            ProtocolNumber = protocol.ProtocolNumber,
            UltrasoundMachine = protocol.UltrasoundMachine,
            Probe = protocol.Probe,
            UltrasoundExamNumber = protocol.UltrasoundExamNumber,
            PatientWeightKg = protocol.PatientWeightKg,
            PatientHeightCm = protocol.PatientHeightCm,
            Symmetry = protocol.Symmetry,
            Right = Deserialize<BreastSideDto>(protocol.RightBreastJson),
            Left = Deserialize<BreastSideDto>(protocol.LeftBreastJson),
            Lesion = Deserialize<BreastLesionDto>(protocol.LesionJson),
            Cysts = Deserialize<BreastCystsDto>(protocol.CystsJson),
            RegionalLymphNodes = Deserialize<RegionalLymphNodesDto>(protocol.RegionalLymphNodesJson),
            AdditionalInfo = protocol.AdditionalInfo,
            Conclusion = protocol.Conclusion,
            Birads = protocol.Birads,
            Recommendations = protocol.Recommendations
        };
    }

    private static string BuildFindings(BreastProtocolCreateDto dto)
    {
        var sb = new StringBuilder();

        AppendLine(sb, "Simmetrikligi", dto.Symmetry);
        AppendSide(sb, "O'ng sut bezi", dto.Right);
        AppendSide(sb, "Chap sut bezi", dto.Left);
        AppendLesion(sb, dto.Lesion);
        AppendCysts(sb, dto.Cysts);
        AppendLymphNodes(sb, dto.RegionalLymphNodes);
        AppendLine(sb, "Qo'shimcha ma'lumotlar", dto.AdditionalInfo);

        return sb.Length == 0
            ? "Sut bezlari bo'yicha patologik UZ-belgilar kiritilmadi."
            : sb.ToString().Trim();
    }

    private static string BuildConclusion(BreastProtocolCreateDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Conclusion))
            return AppendBirads(dto.Conclusion.Trim(), dto.Birads);

        var hasLesion = dto.Lesion.Detected;
        var hasFocal = IsDetected(dto.Right.FocalChanges) || IsDetected(dto.Left.FocalChanges);
        var hasDuctDilation = IsPositive(dto.Right.Ducts, "Kengaygan") || IsPositive(dto.Left.Ducts, "Kengaygan");

        var conclusion = (hasLesion, hasFocal, hasDuctDilation) switch
        {
            (true, _, _) => "Sut bezida o'choqli hosilaning UZ-belgilari.",
            (_, true, _) => "Sut bezlarida o'choqli o'zgarishlarning UZ-belgilari.",
            (_, _, true) => "Sut bezlari yo'llari kengayishining UZ-belgilari.",
            _ => "Sut bezlarida o'choqli patologiyaning UZ-belgilari aniqlanmadi."
        };

        return AppendBirads(conclusion, dto.Birads);
    }

    private static string AppendBirads(string conclusion, string? birads)
    {
        return string.IsNullOrWhiteSpace(birads)
            ? conclusion
            : $"{conclusion} BI-RADS {birads.Trim()}.";
    }

    private static void AppendSide(StringBuilder sb, string title, BreastSideDto side)
    {
        var lines = new List<string>();
        Add(lines, "teri", Combine(side.SkinLine, FormatMm(side.SkinThicknessMm)));
        Add(lines, "strukturasi", side.Structure);
        Add(lines, "so'rg'ich", side.Nipple);
        Add(lines, "so'rg'ich orqa zonasi", Combine(side.RetroareolarZone, side.RetroareolarEchostructure));
        Add(lines, "to'qimalarni farqlash", side.TissueDifferentiation);
        Add(lines, "teri osti yog' to'qimasi", Combine(side.SubcutaneousFat, side.SubcutaneousFatEchostructure));
        Add(lines, "fibroglandulyar kompleks", FormatMm(side.FibroglandularComplexThicknessMm));
        Add(lines, "to'qimalar nisbati", side.TissueRatio);
        Add(lines, "exostruktura", side.Echostructure);
        Add(lines, "o'choqli o'zgarishlar", side.FocalChanges);
        Add(lines, "sut yo'llari", side.Ducts);
        Add(lines, "sut yo'llari devori", side.DuctWall);
        Add(lines, "sut yo'llari tarkibi", side.DuctContent);
        Add(lines, "retromammar yog' qatlami", side.RetromammaryFat);
        Add(lines, "intramammar limfa tugunlari", side.IntramammaryLymphNodes);

        if (lines.Count == 0)
            return;

        sb.AppendLine(title + ": " + string.Join("; ", lines) + ".");
    }

    private static void AppendLesion(StringBuilder sb, BreastLesionDto lesion)
    {
        if (!lesion.Detected)
            return;

        var parts = new List<string>();
        Add(parts, "sut bezi", lesion.Breast);
        Add(parts, "zona", lesion.Zone);
        Add(parts, "o'lchamlari", lesion.SizeMm);
        Add(parts, "shakli", lesion.Shape);
        Add(parts, "joylashuv yo'nalishi", lesion.Orientation);
        Add(parts, "konturlari", lesion.Contours);
        Add(parts, "tuzilmasi", lesion.Structure);
        Add(parts, "exogenligi", lesion.Echogenicity);
        Add(parts, "distal akustik ta'siri", lesion.DistalAcousticEffect);
        Add(parts, "kalsinatlar", Combine(lesion.Calcifications, FormatMm(lesion.CalcificationSizeMm)));
        Add(parts, "qo'shimcha", lesion.AdditionalChanges);

        sb.AppendLine("Hosila: " + string.Join("; ", parts) + ".");
    }

    private static void AppendCysts(StringBuilder sb, BreastCystsDto cysts)
    {
        var rows = new List<string>();
        AddCyst(rows, "tepa-tashqi kvadrant", cysts.UpperOuterRight, cysts.UpperOuterLeft);
        AddCyst(rows, "tepa-ichki kvadrant", cysts.UpperInnerRight, cysts.UpperInnerLeft);
        AddCyst(rows, "pastki-tashqi kvadrant", cysts.LowerOuterRight, cysts.LowerOuterLeft);
        AddCyst(rows, "pastki-ichki kvadrant", cysts.LowerInnerRight, cysts.LowerInnerLeft);
        AddCyst(rows, "tashqi kvadrantlar chegarasi", cysts.OuterBorderRight, cysts.OuterBorderLeft);
        AddCyst(rows, "tepa kvadrantlar chegarasi", cysts.UpperBorderRight, cysts.UpperBorderLeft);
        AddCyst(rows, "ichki kvadrantlar chegarasi", cysts.InnerBorderRight, cysts.InnerBorderLeft);
        AddCyst(rows, "pastki kvadrantlar chegarasi", cysts.LowerBorderRight, cysts.LowerBorderLeft);

        if (rows.Count > 0)
            sb.AppendLine("Kistalar: " + string.Join("; ", rows) + ".");
    }

    private static void AppendLymphNodes(StringBuilder sb, RegionalLymphNodesDto lymphNodes)
    {
        var parts = new List<string>();
        Add(parts, "o'ngdan", lymphNodes.RightSizeMm);
        Add(parts, "chapdan", lymphNodes.LeftSizeMm);
        Add(parts, "soni", lymphNodes.Count);
        Add(parts, "tuzilmasi", lymphNodes.Structure);

        if (parts.Count > 0)
            sb.AppendLine("Regional limfa tugunlari: " + string.Join("; ", parts) + ".");
    }

    private static void AddCyst(List<string> rows, string label, string? right, string? left)
    {
        var values = new List<string>();
        if (!string.IsNullOrWhiteSpace(right))
            values.Add($"o'ngda {right.Trim()}");
        if (!string.IsNullOrWhiteSpace(left))
            values.Add($"chapda {left.Trim()}");

        if (values.Count > 0)
            rows.Add($"{label}: {string.Join(", ", values)}");
    }

    private static void Add(List<string> list, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            list.Add($"{label} - {value.Trim()}");
    }

    private static void AppendLine(StringBuilder sb, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            sb.AppendLine($"{label}: {value.Trim()}.");
    }

    private static string? Combine(params string?[] values)
    {
        var clean = values.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v!.Trim()).ToArray();
        return clean.Length == 0 ? null : string.Join(", ", clean);
    }

    private static string? FormatMm(decimal? value)
    {
        return value.HasValue
            ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) + " mm"
            : null;
    }

    private static bool IsDetected(string? value) =>
        value?.Contains("Aniqlandi", StringComparison.OrdinalIgnoreCase) == true;

    private static bool IsPositive(string? value, string positiveValue) =>
        string.Equals(value?.Trim(), positiveValue, StringComparison.OrdinalIgnoreCase);

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static T Deserialize<T>(string json) where T : new()
    {
        if (string.IsNullOrWhiteSpace(json))
            return new T();

        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? new T();
        }
        catch (JsonException)
        {
            return new T();
        }
    }
}
