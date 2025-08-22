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
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Kişi oluşturuldu");
    }

    public async Task<SuccessResponse> UpdateContactAsync(Guid id, UpdateContactRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Contact/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Kişi güncellendi");
    }

    public async Task<SuccessResponse> DeleteContactAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Contact/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Kişi silindi");
    }

    public async Task<SuccessResponse> AddCommunicationInfoAsync(Guid contactId, AddCommunicationInfoRequest request)
    {
        var createRequest = new CreateCommunicationInfoRequest
        {
            ContactId = contactId,
            Type = request.Type,
            Value = request.Value
        };
        
        var response = await _httpClient.PostAsJsonAsync("api/CommunicationInfo", createRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("İletişim bilgisi eklendi");
    }

    public async Task<SuccessResponse> RemoveCommunicationInfoAsync(Guid contactId, Guid communicationInfoId)
    {
        var response = await _httpClient.DeleteAsync($"api/CommunicationInfo/{contactId}/{communicationInfoId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("İletişim bilgisi silindi");
    }
}
