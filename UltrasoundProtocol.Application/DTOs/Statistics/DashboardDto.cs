namespace UltrasoundProtocol.Application.DTOs.Statistics;

public class DashboardDto
{
    public int TotalPatients { get; set; }
    public int TotalProtocols { get; set; }
    public int ActiveProtocols { get; set; }
    public int CompletedProtocols { get; set; }
    public int TodayAppointments { get; set; }
    public int UnreadNotifications { get; set; }
    public List<MonthlyStatDto> MonthlyStats { get; set; } = [];
}

public class MonthlyStatDto
{
    public string Month { get; set; } = string.Empty;
    public int ProtocolCount { get; set; }
    public int PatientCount { get; set; }
}
