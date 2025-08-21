using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.MessageBus.Models;

public class ReportRequestMessage
{
    public Guid ReportId { get; set; }
    public ReportType ReportType { get; set; }
    public string Parameters { get; set; } = string.Empty; // JSON string
    public DateTime RequestedAt { get; set; }
}
