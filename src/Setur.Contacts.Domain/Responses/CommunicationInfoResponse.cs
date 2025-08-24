using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Responses;

public class CommunicationInfoResponse
{
    public Guid Id { get; set; }
    public CommunicationType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}