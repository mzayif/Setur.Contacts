using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Models;

/// <summary>
/// Redis cache'de saklanan rapor verisi
/// </summary>
public class ReportCacheData
{
    public Guid ReportId { get; set; }
    public ReportType ReportType { get; set; }
    public string Parameters { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<ReportDetailCacheData> Details { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Redis cache'de saklanan rapor detay verisi
/// </summary>
public class ReportDetailCacheData
{
    public string Location { get; set; } = string.Empty;
    public int PersonCount { get; set; }
    public int PhoneCount { get; set; }
    public int EmailCount { get; set; }
}

