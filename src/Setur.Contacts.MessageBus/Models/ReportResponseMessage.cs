using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.MessageBus.Models;

public class ReportResponseMessage
{
    public Guid ReportId { get; set; }
    public ReportStatus Status { get; set; }
    public string Summary { get; set; } = string.Empty; // JSON string - özet bilgiler
    public DateTime CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
