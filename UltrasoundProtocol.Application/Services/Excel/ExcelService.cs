using ClosedXML.Excel;
using UltrasoundProtocol.Application.DTOs.Patient;
using UltrasoundProtocol.Application.DTOs.Protocol;

namespace UltrasoundProtocol.Application.Services.Excel;

public interface IExcelService
{
    byte[] ExportPatients(IEnumerable<PatientDto> patients);
    byte[] ExportProtocols(IEnumerable<ProtocolDto> protocols);
}

public class ExcelService : IExcelService
{
    public byte[] ExportPatients(IEnumerable<PatientDto> patients)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Bemorlar");

        ws.Cell(1, 1).Value = "Ism";
        ws.Cell(1, 2).Value = "Tug'ilgan sana";
        ws.Cell(1, 3).Value = "Jins";
        ws.Cell(1, 4).Value = "Telefon";
        ws.Cell(1, 5).Value = "Email";
        ws.Cell(1, 6).Value = "Holat";
        ws.Cell(1, 7).Value = "Yaratilgan";

        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        var row = 2;
        foreach (var p in patients)
        {
            ws.Cell(row, 1).Value = p.FullName;
            ws.Cell(row, 2).Value = p.DateOfBirth.ToString("dd.MM.yyyy");
            ws.Cell(row, 3).Value = p.Gender;
            ws.Cell(row, 4).Value = p.PhoneNumber;
            ws.Cell(row, 5).Value = p.Email ?? "";
            ws.Cell(row, 6).Value = p.IsActive ? "Faol" : "Nofaol";
            ws.Cell(row, 7).Value = p.CreatedDate.ToString("dd.MM.yyyy");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportProtocols(IEnumerable<ProtocolDto> protocols)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Protokollar");

        ws.Cell(1, 1).Value = "Bemor";
        ws.Cell(1, 2).Value = "Organ";
        ws.Cell(1, 3).Value = "Tekshiruv sanasi";
        ws.Cell(1, 4).Value = "Shifokor";
        ws.Cell(1, 5).Value = "Topilmalar";
        ws.Cell(1, 6).Value = "Xulosa";
        ws.Cell(1, 7).Value = "Holat";

        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightGreen;

        var row = 2;
        foreach (var p in protocols)
        {
            ws.Cell(row, 1).Value = p.PatientName;
            ws.Cell(row, 2).Value = p.BodyPart;
            ws.Cell(row, 3).Value = p.ExamDate.ToString("dd.MM.yyyy");
            ws.Cell(row, 4).Value = p.DoctorDisplayName;
            ws.Cell(row, 5).Value = p.Findings;
            ws.Cell(row, 6).Value = p.Conclusion;
            ws.Cell(row, 7).Value = p.Status.ToString();
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
