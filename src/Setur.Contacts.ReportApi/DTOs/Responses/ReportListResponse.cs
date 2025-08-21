using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.DTOs.Responses;

public class ReportListResponse
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ReportType Type { get; set; }
}
