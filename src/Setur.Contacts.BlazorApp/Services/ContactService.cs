using Setur.Contacts.Base.Results;
using Setur.Contacts.BlazorApp.Services.Abstracts;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SuccessDataResult<IEnumerable<ContactResponse>>> GetAllContactsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<IEnumerable<ContactResponse>>>("api/Contact");
        return response ?? new SuccessDataResult<IEnumerable<ContactResponse>>(new List<ContactResponse>());
    }

    public async Task<SuccessDataResult<ContactDetailResponse?>> GetContactByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<ContactDetailResponse?>>($"api/Contact/{id}");
        return response ?? new SuccessDataResult<ContactDetailResponse?>(null);
    }

    public async Task<SuccessResponse> CreateContactAsync(CreateContactRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Contact", request);
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }

    public async Task<SuccessResponse> UpdateContactAsync(Guid id, UpdateContactRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Contact/{id}", request);
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }

    public async Task<SuccessResponse> DeleteContactAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Contact/{id}");
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }


}
