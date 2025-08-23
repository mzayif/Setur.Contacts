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

    /// <summary>
    /// Kişileri sayfalama ile getirir
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası</param>
    /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
    /// <returns>Sayfalanmış kişi listesi</returns>
    Task<PagedResult<ContactResponse>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
}
