using Setur.Contacts.Base.Domains.Entities;

namespace Setur.Contacts.Domain.Entities;

public class Contact : AddableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Company { get; set; }
    public ICollection<CommunicationInfo>? CommunicationInfos { get; set; }
}
