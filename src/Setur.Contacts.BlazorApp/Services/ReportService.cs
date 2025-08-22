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

    public async Task<SuccessDataResult<ReportResponse>> CreateReportAsync(CreateReportRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Report", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessDataResult<ReportResponse>>() ?? new SuccessDataResult<ReportResponse>(new ReportResponse());
    }

    public async Task<SuccessDataResult<ReportResponse?>> GetReportByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<ReportResponse?>>($"api/Report/{id}");
        return response ?? new SuccessDataResult<ReportResponse?>(null);
    }

    public async Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<IEnumerable<ReportListResponse>>>("api/Report");
        return response ?? new SuccessDataResult<IEnumerable<ReportListResponse>>(new List<ReportListResponse>());
    }

    public async Task<SuccessResponse> DeleteReportAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Report/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SuccessResponse>() ?? new SuccessResponse("Rapor silindi");
    }

    public async Task<SuccessDataResult<ReportDetailResponse?>> GetReportDetailsAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<SuccessDataResult<ReportDetailResponse?>>($"api/Report/{id}/details");
        return response ?? new SuccessDataResult<ReportDetailResponse?>(null);
    }
}
