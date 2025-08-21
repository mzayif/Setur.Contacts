using Setur.Contacts.Base.Domains.Entities;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Entities;
public class Report : AddableEntity
{
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ReportType Type { get; set; }
    public string Parameters { get; set; } = string.Empty; // JSON string - rapor parametreleri
    public string Summary { get; set; } = string.Empty;   // JSON string - özet bilgiler
    public ICollection<ReportDetail> ReportDetails { get; set; } = new List<ReportDetail>();
}
