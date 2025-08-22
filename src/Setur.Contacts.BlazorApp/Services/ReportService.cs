using Setur.Contacts.Base.Results;
using Setur.Contacts.BlazorApp.Services.Abstracts;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services;

public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;

    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SuccessResponse> CreateReportAsync(CreateReportRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Report", request);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Rapor oluşturuldu");
        return result;
    }

    public async Task<SuccessDataResult<ReportSmartResponse?>> GetReportByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<ReportSmartResponse?>>($"api/Report/{id}");
        return response ?? new SuccessDataResult<ReportSmartResponse?>(null);
    }

    public async Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<IEnumerable<ReportListResponse>>>("api/Report");
        return response ?? new SuccessDataResult<IEnumerable<ReportListResponse>>(new List<ReportListResponse>());
    }

    public async Task<SuccessResponse> DeleteReportAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Report/{id}");
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Rapor silindi");
    }

    public async Task<SuccessDataResult<ReportDetailResponse?>> GetReportDetailsAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<ReportDetailResponse?>>($"api/Report/{id}/details");
        return response ?? new SuccessDataResult<ReportDetailResponse?>(null);
    }

    public async Task<SuccessResponse> RetryReportAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/Report/{id}/retry", null);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Rapor yeniden işlemeye gönderildi");
        return result;
    }

    public async Task<SuccessResponse> SaveReportPermanentlyAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/Report/{id}/save-permanently", null);
        var result = await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Rapor kalıcı olarak kaydedildi");
        return result;
    }
}
