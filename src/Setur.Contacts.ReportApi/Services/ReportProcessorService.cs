using Newtonsoft.Json;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.ReportApi.Data;
using Setur.Contacts.ReportApi.Models;
using Setur.Contacts.ReportApi.Repositories;

namespace Setur.Contacts.ReportApi.Services;

public class ReportProcessorService : IReportProcessorService
{
    private readonly ReportRepository _reportRepository;
    private readonly ReportDbContext _context;
    private readonly ILoggerService _loggerService;
    private readonly IReportCacheService _cacheService;

    public ReportProcessorService(
        ReportRepository reportRepository,
        ReportDbContext context,
        ILoggerService loggerService,
        IReportCacheService cacheService)
    {
        _reportRepository = reportRepository;
        _context = context;
        _loggerService = loggerService;
        _cacheService = cacheService;
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
            }

            report.Status = ReportStatus.Preparing;
            await _reportRepository.SaveAsync();

            // Rapor türüne göre işlem yap
            var reportData = await GenerateReportDataAsync(reportType, parameters);

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

    private Task<ReportCacheData> GenerateReportDataAsync(ReportType reportType, string parameters)
    {
        // Bu kısım gerçekte ContactApi'den veri çekecek
        // Şimdilik simüle edilmiş veriler döndürüyoruz

        var reportData = new ReportCacheData
        {
            ReportId = Guid.NewGuid(),
            ReportType = reportType,
            Parameters = parameters,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        switch (reportType)
        {
            case ReportType.LocationBased:
                var locationParams = JsonConvert.DeserializeObject<LocationReportParameters>(parameters);
                reportData.Summary = JsonConvert.SerializeObject(new
                {
                    location = locationParams?.Location ?? "Bilinmeyen",
                    totalPersonCount = Random.Shared.Next(50, 200),
                    totalPhoneCount = Random.Shared.Next(30, 150),
                    totalEmailCount = Random.Shared.Next(20, 100),
                    totalLocationCount = Random.Shared.Next(1, 10)
                });

                // Detay verileri
                reportData.Details = new List<ReportDetailCacheData>
                {
                    new() { Location = "Kadıköy", PersonCount = 45, PhoneCount = 38, EmailCount = 30},
                    new() { Location = "Beşiktaş", PersonCount = 32, PhoneCount = 28, EmailCount = 22 },
                    new() { Location = "Şişli", PersonCount = 28, PhoneCount = 25, EmailCount = 18 }
                };
                break;

            case ReportType.CompanyBased:
                var companyParams = JsonConvert.DeserializeObject<CompanyReportParameters>(parameters);
                reportData.Summary = JsonConvert.SerializeObject(new
                {
                    company = companyParams?.Company ?? "Bilinmeyen",
                    totalPersonCount = Random.Shared.Next(10, 100),
                    totalPhoneCount = Random.Shared.Next(8, 80),
                    totalEmailCount = Random.Shared.Next(5, 60),
                    locationCount = Random.Shared.Next(1, 5)
                });

                // Detay verileri
                reportData.Details = new List<ReportDetailCacheData>
                {
                    new() { Location = "İstanbul", PersonCount = 25, PhoneCount = 22, EmailCount = 18 },
                    new() { Location = "Ankara", PersonCount = 18, PhoneCount = 15, EmailCount = 12 },
                    new() { Location = "İzmir", PersonCount = 12, PhoneCount = 10, EmailCount = 8 }
                };
                break;

            case ReportType.General:
                reportData.Summary = JsonConvert.SerializeObject(new
                {
                    totalContacts = Random.Shared.Next(500, 2000),
                    totalPhones = Random.Shared.Next(400, 1600),
                    totalEmails = Random.Shared.Next(300, 1200),
                    totalLocations = Random.Shared.Next(10, 50),
                    topCompanies = new[] { "Setur", "ABC Ltd", "XYZ Corp" },
                    topLocations = new[] { "İstanbul", "Ankara", "İzmir" }
                });

                // Detay verileri
                reportData.Details = new List<ReportDetailCacheData>
                {
                    new() { Location = "İstanbul", PersonCount = 150, PhoneCount = 120, EmailCount = 80 },
                    new() { Location = "Ankara", PersonCount = 85, PhoneCount = 70, EmailCount = 45 },
                    new() { Location = "İzmir", PersonCount = 65, PhoneCount = 55, EmailCount = 35 }
                };
                break;

            default:
                reportData.Summary = JsonConvert.SerializeObject(new { error = "Bilinmeyen rapor türü" });
                break;
        }

        return Task.FromResult(reportData);
    }

    private class LocationReportParameters
    {
        public string? Location { get; set; }
    }

    private class CompanyReportParameters
    {
        public string? Company { get; set; }
    }
}
