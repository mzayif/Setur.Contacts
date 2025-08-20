namespace Setur.Contacts.Domain.Entities
{
    public class ReportDetail
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string Location { get; set; } = null!;
        public int PersonCount { get; set; }
        public int PhoneCount { get; set; }
        public Report Report { get; set; } = null!;
    }
}
