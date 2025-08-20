namespace Setur.Contacts.ContactApi.DTOs.Responses
{
    public class ContactDetailResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public List<CommunicationInfoResponse> CommunicationInfos { get; set; }
    }
}
