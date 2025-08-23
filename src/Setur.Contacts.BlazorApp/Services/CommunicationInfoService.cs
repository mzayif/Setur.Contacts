using Setur.Contacts.Base.Results;
using Setur.Contacts.BlazorApp.Services.Abstracts;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services;

public class CommunicationInfoService : ICommunicationInfoService
{
    private readonly HttpClient _httpClient;

    public CommunicationInfoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetAllCommunicationInfosAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>>("api/CommunicationInfo");
        return response ?? new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(new List<CommunicationInfoResponse>());
    }

    public async Task<SuccessDataResult<CommunicationInfoResponse?>> GetCommunicationInfoByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<CommunicationInfoResponse?>>($"api/CommunicationInfo/{id}");
        return response ?? new SuccessDataResult<CommunicationInfoResponse?>(null);
    }

    public async Task<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>> GetCommunicationInfosByContactIdAsync(Guid contactId)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<IEnumerable<CommunicationInfoResponse>>>($"api/CommunicationInfo/contacts/{contactId}");
        return response ?? new SuccessDataResult<IEnumerable<CommunicationInfoResponse>>(new List<CommunicationInfoResponse>());
    }

    public async Task<SuccessResponse> CreateCommunicationInfoAsync(CreateCommunicationInfoRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/CommunicationInfo", request);
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }

    public async Task<SuccessResponse> UpdateCommunicationInfoAsync(Guid id, UpdateCommunicationInfoRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/CommunicationInfo/{id}", request);
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }

    public async Task<SuccessResponse> DeleteCommunicationInfoAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/CommunicationInfo/{id}");
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new ErrorResponse("Servis Hatası");
    }
}
