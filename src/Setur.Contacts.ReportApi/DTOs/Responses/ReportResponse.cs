using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.DTOs.Responses;

public class ReportResponse
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public List<ReportDetailResponse> ReportDetails { get; set; } = new List<ReportDetailResponse>();
}
