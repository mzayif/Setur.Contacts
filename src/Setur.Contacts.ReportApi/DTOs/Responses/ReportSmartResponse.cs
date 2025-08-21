using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.DTOs.Responses;

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
    
    // Detay sayısı
    public int DetailCount => Details?.Count ?? 0;
    
    // Veri durumu
    public bool HasDetails => DetailCount > 0;
    public bool IsFromCache => DataSource.Contains("Cache");
    public bool IsFromDatabase => DataSource.Contains("Database");
    public bool IsMetadataOnly => DataSource.Contains("Metadata");
}
