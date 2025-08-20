namespace Setur.Contacts.Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Company { get; set; }
        public ICollection<CommunicationInfo>? CommunicationInfos { get; set; }
    }
}
