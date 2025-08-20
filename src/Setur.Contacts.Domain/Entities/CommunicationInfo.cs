using Setur.Contacts.Base.Domains.Entities;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Entities;

public class CommunicationInfo : AddableEntity
{
    public Guid ContactId { get; set; }
    public CommunicationType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public Contact? Contact { get; set; }
}
