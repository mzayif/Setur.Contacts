using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services.Abstracts;

public interface IContactService
{
    Task<SuccessDataResult<IEnumerable<ContactResponse>>> GetAllContactsAsync();
    Task<SuccessDataResult<ContactDetailResponse?>> GetContactByIdAsync(Guid id);
    Task<SuccessResponse> CreateContactAsync(CreateContactRequest request);
    Task<SuccessResponse> UpdateContactAsync(Guid id, UpdateContactRequest request);
    Task<SuccessResponse> DeleteContactAsync(Guid id);
    Task<SuccessResponse> AddCommunicationInfoAsync(Guid contactId, AddCommunicationInfoRequest request);
    Task<SuccessResponse> RemoveCommunicationInfoAsync(Guid contactId, Guid communicationInfoId);
}
