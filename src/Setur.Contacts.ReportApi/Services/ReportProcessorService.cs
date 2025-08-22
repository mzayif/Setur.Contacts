using Newtonsoft.Json;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.CommonModels;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Responses;
using Setur.Contacts.ReportApi.Data;
using Setur.Contacts.ReportApi.Repositories;

namespace Setur.Contacts.ReportApi.Services;

public class ReportProcessorService : IReportProcessorService
{
    private readonly ReportRepository _reportRepository;
    private readonly ReportDbContext _context;
    private readonly ILoggerService _loggerService;
    private readonly IReportCacheService _cacheService;
    private readonly HttpClient _httpClient;
    private readonly string _contactApiBaseUrl;

    public ReportProcessorService(
        ReportRepository reportRepository,
        ReportDbContext context,
        ILoggerService loggerService,
        IReportCacheService cacheService,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _reportRepository = reportRepository;
        _context = context;
        _loggerService = loggerService;
        _cacheService = cacheService;
        _httpClient = httpClient;
        _contactApiBaseUrl = configuration["ContactApiBaseUrl"] ?? "https://localhost:7001";
    }

    public async Task ProcessReportAsync(Guid reportId, ReportType reportType, string parameters)
    {
        try
        {
            _loggerService.LogInformation($"Rapor işleme başladı. ReportId: {reportId}, Type: {reportType}");

            // Rapor durumunu "Hazırlanıyor" olarak güncelle
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
            {
                _loggerService.LogError($"Rapor bulunamadı. ReportId: {reportId}");
                return;
            }

            report.Status = ReportStatus.Preparing;
            await _reportRepository.SaveAsync();

            // ContactApi'den gerçek veri çek
            var reportData = await GenerateReportDataFromContactApiAsync(reportType, parameters);

            // Cache'e kaydet
            await _cacheService.SetReportAsync(reportId, reportData);

            // Raporu tamamla
            report.Status = ReportStatus.Completed;
            await _reportRepository.SaveAsync();

            _loggerService.LogInformation($"Rapor tamamlandı. ReportId: {reportId}");
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Rapor işleme hatası. ReportId: {reportId}, Error: {ex.Message}");

            // Hata durumunda raporu güncelle
            var report = await _reportRepository.GetByIdAsync(reportId, throwException: false);
            if (report != null)
            {
                report.Status = ReportStatus.Failed;
                await _reportRepository.SaveAsync();
            }
        }
    }

    private async Task<ReportCacheData> GenerateReportDataFromContactApiAsync(ReportType reportType, string parameters)
    {
        var reportData = new ReportCacheData
        {
            ReportId = Guid.NewGuid(),
            ReportType = reportType,
            Parameters = parameters,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        var endpoint = reportType switch
        {
            ReportType.LocationBased => "location",
            ReportType.CompanyBased => "company",
            ReportType.General => "general",
            _ => "general"
        };

        var url = $"{_contactApiBaseUrl}/api/ReportData/{endpoint}";

        // Parameters'dan filtreleri çıkar
        if (!string.IsNullOrEmpty(parameters) && reportType != ReportType.General)
        {
            var filters = parameters.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f))
                .ToList();

            if (filters.Any())
            {
                var filterString = string.Join(",", filters);
                url += $"?{endpoint}s={Uri.EscapeDataString(filterString)}";
            }
        }

        _loggerService.LogInformation($"ContactApi'ye istek gönderiliyor: {url}");

        var response = await _httpClient.GetAsync(url);
        var responseData = await response.Content.ReadAsStringAsync();

        _loggerService.LogInformation($"ContactApi yanıtı: Status={response.StatusCode}, Data={responseData}");

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<SuccessDataResult<ReportDataResponse>>(responseData);

            if (result?.Data != null)
            {
                reportData.Summary = JsonConvert.SerializeObject(new
                {
                    reportType = result.Data.ReportType.ToString(),
                    filters = result.Data.Filters,
                    totalPersonCount = result.Data.TotalPersonCount,
                    totalPhoneCount = result.Data.TotalPhoneCount,
                    totalEmailCount = result.Data.TotalEmailCount,
                    totalLocationCount = result.Data.TotalLocationCount,
                    topCompanies = result.Data.TopCompanies,
                    topLocations = result.Data.TopLocations
                });

                reportData.Details = result.Data.Details.Select(d => new ReportDetailCacheData
                {
                    Location = d.Location,
                    PersonCount = d.PersonCount,
                    PhoneCount = d.PhoneCount,
                    EmailCount = d.EmailCount
                }).ToList();
            }
            else
            {
                throw new Exception("ContactApi'den boş veri döndü");
            }
        }
        else
        {
            throw new Exception($"ContactApi hatası: {response.StatusCode} - {responseData}");
        }

        return reportData;
    }
}


