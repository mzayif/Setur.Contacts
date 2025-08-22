using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Responses;

public class ReportResponse
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ReportType Type { get; set; }
    public string Parameters { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<ReportDetailResponse> ReportDetails { get; set; } = new List<ReportDetailResponse>();
}
