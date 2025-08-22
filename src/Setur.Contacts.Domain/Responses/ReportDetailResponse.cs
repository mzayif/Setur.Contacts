namespace Setur.Contacts.Domain.Responses;

public class ReportDetailResponse
{
    public Guid Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public int PersonCount { get; set; }
    public int PhoneCount { get; set; }
    public int EmailCount { get; set; }
}
