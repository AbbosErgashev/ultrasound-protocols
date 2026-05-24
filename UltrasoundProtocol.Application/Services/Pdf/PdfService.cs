using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UltrasoundProtocol.Application.DTOs.BreastProtocol;
using UltrasoundProtocol.Application.DTOs.Protocol;
using UltrasoundProtocol.Domain.Enums;

namespace UltrasoundProtocol.Application.Services.Pdf;

public interface IPdfService
{
    byte[] GenerateProtocolPdf(ProtocolDto protocol);
    byte[] GenerateBreastProtocolPdf(ProtocolDto protocol, BreastProtocolPdfDto breastProtocol);
}

public class PdfService : IPdfService
{
    public byte[] GenerateProtocolPdf(ProtocolDto protocol)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, protocol));
                page.Content().Element(c => ComposeContent(c, protocol));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateBreastProtocolPdf(ProtocolDto protocol, BreastProtocolPdfDto breastProtocol)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(26);
                page.DefaultTextStyle(x => x.FontSize(9).LineHeight(1.25f));

                page.Header().Element(c => ComposeBreastHeader(c, protocol, breastProtocol));
                page.Content().Element(c => ComposeBreastContent(c, protocol, breastProtocol));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, ProtocolDto protocol)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("MedUZI Diagnostika Markazi")
                        .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().Text("Toshkent sh., Chilonzor t., Bunyodkor ko'chasi, 42")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                    c.Item().Text("Tel: +998 71 200-00-01")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(120).AlignRight().Column(c =>
                {
                    c.Item().Text($"#{protocol.Id.ToString()[..8]}")
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"{protocol.CreatedAt:dd.MM.yyyy}")
                        .FontSize(9);
                });
            });

            col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Blue.Darken2);

            col.Item().AlignCenter().Text("ULTRATOVUSH TEKSHIRUVI PROTOKOLI")
                .FontSize(14).Bold();
        });
    }

    private static void ComposeContent(IContainer container, ProtocolDto protocol)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Spacing(8);

            col.Item().Element(c => InfoRow(c, "Bemor:", ValueOrDefault(protocol.PatientName, "Kiritilmagan")));
            col.Item().Element(c => InfoRow(c, "Tekshirilgan organ:", protocol.BodyPart));
            col.Item().Element(c => InfoRow(c, "Tekshiruv sanasi:", protocol.ExamDate.ToString("dd.MM.yyyy")));
            col.Item().Element(c => InfoRow(c, "Shifokor:", protocol.DoctorUsername));
            col.Item().Element(c => InfoRow(c, "Holat:", TranslateStatus(protocol.Status)));

            col.Item().PaddingTop(10).Text("TOPILMALAR")
                .FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
            col.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(8).Text(protocol.Findings).FontSize(10).LineHeight(1.5f);

            col.Item().PaddingTop(10).Text("XULOSA")
                .FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
            col.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(8).Text(protocol.Conclusion).FontSize(10).LineHeight(1.5f);

            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().LineHorizontal(1);
                    c.Item().Text("Shifokor imzosi").FontSize(9).Italic();
                });
                row.ConstantItem(100);
                row.RelativeItem().Column(c =>
                {
                    c.Item().LineHorizontal(1);
                    c.Item().Text("Sana / Muhr").FontSize(9).Italic();
                });
            });
        });
    }

    private static void ComposeBreastHeader(IContainer container, ProtocolDto protocol, BreastProtocolPdfDto breastProtocol)
    {
        container.Column(col =>
        {
            col.Spacing(5);
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(ValueOrDefault(breastProtocol.MedicalInstitutionName, "MedUZI Diagnostika Markazi"))
                        .FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
                    c.Item().Text(ValueOrDefault(breastProtocol.MedicalInstitutionAddress, "Manzil kiritilmagan"))
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(145).AlignRight().Column(c =>
                {
                    c.Item().Text($"Protokol N: {ValueOrDefault(breastProtocol.ProtocolNumber, protocol.Id.ToString()[..8])}")
                        .FontSize(8).Bold();
                    c.Item().Text($"Sana: {protocol.ExamDate:dd.MM.yyyy}")
                        .FontSize(8);
                });
            });

            col.Item().BorderBottom(1).BorderColor(Colors.Blue.Darken2).PaddingBottom(5)
                .AlignCenter().Text("SUT BEZLARINING ULTRATOVUSH TEKSHIRUVI BAYONNOMASI")
                .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);
        });
    }

    private static void ComposeBreastContent(
        IContainer container,
        ProtocolDto protocol,
        BreastProtocolPdfDto breastProtocol)
    {
        container.PaddingTop(8).Column(col =>
        {
            col.Spacing(8);

            col.Item().Element(c => ComposePatientBlock(c, protocol, breastProtocol));
            col.Item().Element(c => ComposeExamBlock(c, protocol, breastProtocol));

            col.Item().Element(c => SectionTitle(c, "Asosiy ma'lumotlar"));
            col.Item().Element(c => TwoColumnBreastBlock(c, breastProtocol));

            if (breastProtocol.Lesion.Detected || HasAnyLesionValue(breastProtocol.Lesion))
                col.Item().Element(c => ComposeLesionBlock(c, breastProtocol.Lesion));

            if (HasAnyCystValue(breastProtocol.Cysts))
                col.Item().Element(c => ComposeCystsTable(c, breastProtocol.Cysts));

            if (HasAnyLymphNodeValue(breastProtocol.RegionalLymphNodes))
                col.Item().Element(c => ComposeLymphNodesBlock(c, breastProtocol.RegionalLymphNodes));

            if (!string.IsNullOrWhiteSpace(breastProtocol.AdditionalInfo))
                col.Item().Element(c => TextBox(c, "Qo'shimcha ma'lumotlar", breastProtocol.AdditionalInfo!));

            col.Item().Element(c => TextBox(c, "Xulosa", ValueOrDefault(breastProtocol.Conclusion, protocol.Conclusion), true));

            if (!string.IsNullOrWhiteSpace(breastProtocol.Recommendations))
                col.Item().Element(c => TextBox(c, "Tavsiyalar", breastProtocol.Recommendations!));

            col.Item().PaddingTop(18).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Shifokor: {ValueOrDefault(breastProtocol.DoctorName, protocol.DoctorUsername)}").FontSize(9);
                    c.Item().PaddingTop(14).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    c.Item().Text("Imzo").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(80);
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Holat: {TranslateStatus(protocol.Status)}").FontSize(9);
                    c.Item().PaddingTop(14).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    c.Item().Text("Muhr").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        });
    }

    private static void ComposePatientBlock(IContainer container, ProtocolDto protocol, BreastProtocolPdfDto breastProtocol)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            MetaCell(table, "Bemor", ValueOrDefault(protocol.PatientName, "Kiritilmagan"));
            MetaCell(table, "Tekshiruv sanasi", protocol.ExamDate.ToString("dd.MM.yyyy"));
            MetaCell(table, "Shifokor", ValueOrDefault(breastProtocol.DoctorName, protocol.DoctorUsername));
            MetaCell(table, "Vazni", breastProtocol.PatientWeightKg.HasValue ? $"{breastProtocol.PatientWeightKg:0.##} kg" : "-");
            MetaCell(table, "Bo'yi", breastProtocol.PatientHeightCm.HasValue ? $"{breastProtocol.PatientHeightCm:0.##} sm" : "-");
            MetaCell(table, "Simmetrikligi", ValueOrDefault(breastProtocol.Symmetry, "-"));
        });
    }

    private static void ComposeExamBlock(IContainer container, ProtocolDto protocol, BreastProtocolPdfDto breastProtocol)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            MetaCell(table, "Tekshirilgan organ", protocol.BodyPart);
            MetaCell(table, "UZI apparati", ValueOrDefault(breastProtocol.UltrasoundMachine, "-"));
            MetaCell(table, "Datchik", ValueOrDefault(breastProtocol.Probe, "-"));
            MetaCell(table, "UZI tekshiruvi N", ValueOrDefault(breastProtocol.UltrasoundExamNumber, "-"));
            MetaCell(table, "BI-RADS", ValueOrDefault(breastProtocol.Birads, "-"));
            MetaCell(table, "Protokol holati", TranslateStatus(protocol.Status));
        });
    }

    private static void TwoColumnBreastBlock(IContainer container, BreastProtocolPdfDto breastProtocol)
    {
        container.Row(row =>
        {
            row.RelativeItem().Element(c => ComposeBreastSide(c, "O'ng sut bezi", breastProtocol.Right));
            row.ConstantItem(10);
            row.RelativeItem().Element(c => ComposeBreastSide(c, "Chap sut bezi", breastProtocol.Left));
        });
    }

    private static void ComposeBreastSide(IContainer container, string title, BreastSideDto side)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Column(col =>
        {
            col.Item().Background(Colors.Blue.Darken2).PaddingVertical(5).PaddingHorizontal(7)
                .Text(title).Bold().FontColor(Colors.White).FontSize(10);

            col.Item().Padding(7).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(112);
                    columns.RelativeColumn();
                });

                DetailRow(table, "Teri", JoinValues(side.SkinLine, FormatMm(side.SkinThicknessMm)));
                DetailRow(table, "Strukturasi", side.Structure);
                DetailRow(table, "So'rg'ich", side.Nipple);
                DetailRow(table, "So'rg'ich orqa zonasi", JoinValues(side.RetroareolarZone, side.RetroareolarEchostructure));
                DetailRow(table, "To'qimani farqlash", side.TissueDifferentiation);
                DetailRow(table, "Teri osti yog'i", JoinValues(side.SubcutaneousFat, side.SubcutaneousFatEchostructure));
                DetailRow(table, "Fibroglandulyar kompleks", FormatMm(side.FibroglandularComplexThicknessMm));
                DetailRow(table, "To'qimalar nisbati", side.TissueRatio);
                DetailRow(table, "Exostruktura", side.Echostructure);
                DetailRow(table, "O'choqli o'zgarishlar", side.FocalChanges);
                DetailRow(table, "Sut yo'llari", side.Ducts);
                DetailRow(table, "Devori", side.DuctWall);
                DetailRow(table, "Tarkibi", side.DuctContent);
                DetailRow(table, "Retromammar yog'", side.RetromammaryFat);
                DetailRow(table, "Intramammar limfa", side.IntramammaryLymphNodes);
            });
        });
    }

    private static void ComposeLesionBlock(IContainer container, BreastLesionDto lesion)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Hosila"));
            col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                MetaCell(table, "Sut bezi", ValueOrDefault(lesion.Breast, "-"));
                MetaCell(table, "Zona", ValueOrDefault(lesion.Zone, "-"));
                MetaCell(table, "O'lchami", ValueOrDefault(lesion.SizeMm, "-"));
                MetaCell(table, "Shakli", ValueOrDefault(lesion.Shape, "-"));
                MetaCell(table, "Yo'nalishi", ValueOrDefault(lesion.Orientation, "-"));
                MetaCell(table, "Konturlari", ValueOrDefault(lesion.Contours, "-"));
                MetaCell(table, "Tuzilmasi", ValueOrDefault(lesion.Structure, "-"));
                MetaCell(table, "Exogenligi", ValueOrDefault(lesion.Echogenicity, "-"));
                MetaCell(table, "Akustik ta'siri", ValueOrDefault(lesion.DistalAcousticEffect, "-"));
                MetaCell(table, "Kalsinatlar", JoinValues(lesion.Calcifications, FormatMm(lesion.CalcificationSizeMm)));
            });

            if (!string.IsNullOrWhiteSpace(lesion.AdditionalChanges))
                col.Item().PaddingTop(4).Element(c => TextBox(c, "Hosila bo'yicha qo'shimcha", lesion.AdditionalChanges!));
        });
    }

    private static void ComposeCystsTable(IContainer container, BreastCystsDto cysts)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Kistalar"));
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                HeaderCell(table, "Kvadrant");
                HeaderCell(table, "O'ng sut bezi");
                HeaderCell(table, "Chap sut bezi");

                CystRow(table, "Tepa-tashqi", cysts.UpperOuterRight, cysts.UpperOuterLeft);
                CystRow(table, "Tepa-ichki", cysts.UpperInnerRight, cysts.UpperInnerLeft);
                CystRow(table, "Pastki-tashqi", cysts.LowerOuterRight, cysts.LowerOuterLeft);
                CystRow(table, "Pastki-ichki", cysts.LowerInnerRight, cysts.LowerInnerLeft);
                CystRow(table, "Tashqi chegarasi", cysts.OuterBorderRight, cysts.OuterBorderLeft);
                CystRow(table, "Tepa chegarasi", cysts.UpperBorderRight, cysts.UpperBorderLeft);
                CystRow(table, "Ichki chegarasi", cysts.InnerBorderRight, cysts.InnerBorderLeft);
                CystRow(table, "Pastki chegarasi", cysts.LowerBorderRight, cysts.LowerBorderLeft);
            });
        });
    }

    private static void ComposeLymphNodesBlock(IContainer container, RegionalLymphNodesDto lymphNodes)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, "Regional limfa tugunlari"));
            col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                MetaCell(table, "O'ngdan", ValueOrDefault(lymphNodes.RightSizeMm, "-"));
                MetaCell(table, "Chapdan", ValueOrDefault(lymphNodes.LeftSizeMm, "-"));
                MetaCell(table, "Soni", ValueOrDefault(lymphNodes.Count, "-"));
                MetaCell(table, "Tuzilmasi", ValueOrDefault(lymphNodes.Structure, "-"));
            });
        });
    }

    private static void TextBox(IContainer container, string title, string value, bool important = false)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionTitle(c, title));
            col.Item()
                .Border(1)
                .BorderColor(important ? Colors.Blue.Lighten2 : Colors.Grey.Lighten2)
                .Background(important ? Colors.Blue.Lighten5 : Colors.White)
                .Padding(8)
                .Text(value)
                .FontSize(9)
                .LineHeight(1.35f);
        });
    }

    private static void InfoRow(IContainer container, string label, string value)
    {
        container.Row(row =>
        {
            row.ConstantItem(140).Text(label).Bold().FontSize(10);
            row.RelativeItem().Text(value).FontSize(10);
        });
    }

    private static void SectionTitle(IContainer container, string title)
    {
        container.Background(Colors.Grey.Lighten4).PaddingVertical(4).PaddingHorizontal(7)
            .Text(title).Bold().FontSize(10).FontColor(Colors.Blue.Darken3);
    }

    private static void MetaCell(TableDescriptor table, string label, string value)
    {
        table.Cell().PaddingBottom(5).Column(c =>
        {
            c.Item().Text(label).FontSize(7).FontColor(Colors.Grey.Darken1);
            c.Item().Text(value).FontSize(9).Bold();
        });
    }

    private static void DetailRow(TableDescriptor table, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(3)
            .Text(label).FontSize(7).FontColor(Colors.Grey.Darken1);
        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(3)
            .Text(value).FontSize(8);
    }

    private static void HeaderCell(TableDescriptor table, string text)
    {
        table.Cell().Background(Colors.Blue.Darken2).Padding(5)
            .Text(text).Bold().FontColor(Colors.White).FontSize(8);
    }

    private static void CystRow(TableDescriptor table, string label, string? right, string? left)
    {
        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(label).FontSize(8);
        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ValueOrDefault(right, "-")).FontSize(8);
        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ValueOrDefault(left, "-")).FontSize(8);
    }

    private static void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("MedUZI Diagnostika Markazi - sahifa ").FontSize(8).FontColor(Colors.Grey.Medium);
            text.CurrentPageNumber().FontSize(8);
            text.Span(" / ").FontSize(8);
            text.TotalPages().FontSize(8);
        });
    }

    private static string TranslateStatus(ProtocolStatus status) => status switch
    {
        ProtocolStatus.Draft => "Qoralama",
        ProtocolStatus.Active => "Faol",
        ProtocolStatus.Completed => "Yakunlangan",
        ProtocolStatus.Archived => "Arxivlangan",
        _ => status.ToString()
    };

    private static bool HasAnyLesionValue(BreastLesionDto lesion) =>
        !string.IsNullOrWhiteSpace(lesion.Breast) ||
        !string.IsNullOrWhiteSpace(lesion.Zone) ||
        !string.IsNullOrWhiteSpace(lesion.SizeMm) ||
        !string.IsNullOrWhiteSpace(lesion.Shape) ||
        !string.IsNullOrWhiteSpace(lesion.Orientation) ||
        !string.IsNullOrWhiteSpace(lesion.Contours) ||
        !string.IsNullOrWhiteSpace(lesion.Structure) ||
        !string.IsNullOrWhiteSpace(lesion.Echogenicity) ||
        !string.IsNullOrWhiteSpace(lesion.DistalAcousticEffect) ||
        !string.IsNullOrWhiteSpace(lesion.Calcifications) ||
        lesion.CalcificationSizeMm.HasValue ||
        !string.IsNullOrWhiteSpace(lesion.AdditionalChanges);

    private static bool HasAnyCystValue(BreastCystsDto cysts) =>
        new[]
        {
            cysts.UpperOuterRight, cysts.UpperOuterLeft, cysts.UpperInnerRight, cysts.UpperInnerLeft,
            cysts.LowerOuterRight, cysts.LowerOuterLeft, cysts.LowerInnerRight, cysts.LowerInnerLeft,
            cysts.OuterBorderRight, cysts.OuterBorderLeft, cysts.UpperBorderRight, cysts.UpperBorderLeft,
            cysts.InnerBorderRight, cysts.InnerBorderLeft, cysts.LowerBorderRight, cysts.LowerBorderLeft
        }.Any(v => !string.IsNullOrWhiteSpace(v));

    private static bool HasAnyLymphNodeValue(RegionalLymphNodesDto lymphNodes) =>
        !string.IsNullOrWhiteSpace(lymphNodes.RightSizeMm) ||
        !string.IsNullOrWhiteSpace(lymphNodes.LeftSizeMm) ||
        !string.IsNullOrWhiteSpace(lymphNodes.Count) ||
        !string.IsNullOrWhiteSpace(lymphNodes.Structure);

    private static string ValueOrDefault(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static string? FormatMm(decimal? value) =>
        value.HasValue ? $"{value:0.##} mm" : null;

    private static string JoinValues(params string?[] values)
    {
        var clean = values.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v!.Trim()).ToArray();
        return clean.Length == 0 ? "-" : string.Join(", ", clean);
    }
}
