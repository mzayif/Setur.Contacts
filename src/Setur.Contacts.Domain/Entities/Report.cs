using Setur.Contacts.Base.Domains.Entities;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Entities;
public class Report : AddableEntity
{
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ICollection<ReportDetail> ReportDetails { get; set; } = null!;
}
