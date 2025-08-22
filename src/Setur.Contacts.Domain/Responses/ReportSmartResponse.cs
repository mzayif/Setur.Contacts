using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Responses;

/// <summary>
/// Akıllı rapor response modeli - tüm durumları kapsar
/// </summary>
public class ReportSmartResponse
{
    public Guid ReportId { get; set; }
    public ReportType ReportType { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public object Parameters { get; set; } = new();
    public object Summary { get; set; } = new();
    public List<ReportDetailResponse> Details { get; set; } = new();

    // Cache bilgileri (sadece cache'den geldiğinde dolu)
    public DateTime? CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Veri kaynağı bilgisi
    public string DataSource { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
