using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UltrasoundProtocol.Application.DTOs.Protocol;

namespace UltrasoundProtocol.Application.Services.Pdf;

public interface IPdfService
{
    byte[] GenerateProtocolPdf(ProtocolDto protocol, string? aiAnalysis = null);
}

public class PdfService : IPdfService
{
    public byte[] GenerateProtocolPdf(ProtocolDto protocol, string? aiAnalysis = null)
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
                page.Content().Element(c => ComposeContent(c, protocol, aiAnalysis));
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

    private static void ComposeContent(IContainer container, ProtocolDto protocol, string? aiAnalysis)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Spacing(8);

            col.Item().Element(c => InfoRow(c, "Bemor:", protocol.PatientName));
            col.Item().Element(c => InfoRow(c, "Tekshirilgan organ:", protocol.BodyPart));
            col.Item().Element(c => InfoRow(c, "Tekshiruv sanasi:", protocol.ExamDate.ToString("dd.MM.yyyy")));
            col.Item().Element(c => InfoRow(c, "Shifokor:", protocol.DoctorUsername));
            col.Item().Element(c => InfoRow(c, "Holat:", protocol.Status.ToString()));

            col.Item().PaddingTop(10).Text("TOPILMALAR (Findings)")
                .FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
            col.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(8).Text(protocol.Findings).FontSize(10).LineHeight(1.5f);

            col.Item().PaddingTop(10).Text("XULOSA (Conclusion)")
                .FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
            col.Item().PaddingBottom(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(8).Text(protocol.Conclusion).FontSize(10).LineHeight(1.5f);

            if (!string.IsNullOrEmpty(aiAnalysis))
            {
                col.Item().PaddingTop(10).Text("AI TAHLILI")
                    .FontSize(12).Bold().FontColor(Colors.Green.Darken2);
                col.Item().Border(1).BorderColor(Colors.Green.Lighten2)
                    .Background(Colors.Green.Lighten5).Padding(8)
                    .Text(aiAnalysis).FontSize(9).LineHeight(1.4f);
            }

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

    private static void InfoRow(IContainer container, string label, string value)
    {
        container.Row(row =>
        {
            row.ConstantItem(140).Text(label).Bold().FontSize(10);
            row.RelativeItem().Text(value).FontSize(10);
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("MedUZI Diagnostika Markazi — ").FontSize(8).FontColor(Colors.Grey.Medium);
            text.CurrentPageNumber().FontSize(8);
            text.Span(" / ").FontSize(8);
            text.TotalPages().FontSize(8);
        });
    }
}
