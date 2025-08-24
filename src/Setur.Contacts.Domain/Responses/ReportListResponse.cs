using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Responses;

public class ReportListResponse
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ReportType Type { get; set; }
}
