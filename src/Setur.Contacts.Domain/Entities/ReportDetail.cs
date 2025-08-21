using Setur.Contacts.Base.Domains.Entities;

namespace Setur.Contacts.Domain.Entities;

public class ReportDetail : AddableEntity
{
    public Guid ReportId { get; set; }
    public string Location { get; set; } = string.Empty;
    public int PersonCount { get; set; }
    public int PhoneCount { get; set; }
    public int EmailCount { get; set; }
    public int LocationCount { get; set; }
    public Report? Report { get; set; }
}
