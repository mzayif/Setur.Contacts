namespace Setur.Contacts.Domain.Responses;

public class ContactDetailResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public List<CommunicationInfoResponse> CommunicationInfos { get; set; } = new List<CommunicationInfoResponse>();
}