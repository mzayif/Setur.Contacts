using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Entities
{
    public class CommunicationInfo
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public CommunicationType Type { get; set; }
        public string Value { get; set; } = null!;

        public Contact Contact { get; set; }
    }
}
