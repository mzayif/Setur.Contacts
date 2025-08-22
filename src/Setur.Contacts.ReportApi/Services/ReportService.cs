using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;
using Setur.Contacts.MessageBus.Models;
using Setur.Contacts.MessageBus.Services;
using Setur.Contacts.ReportApi.Repositories;

namespace Setur.Contacts.ReportApi.Services;

public class ReportService : IReportService
{
    private readonly ReportRepository _reportRepository;
    private readonly ReportDetailRepository _reportDetailRepository;
    private readonly IReportCacheService _cacheService;
    private readonly IKafkaProducerService _kafkaProducerService;

    public ReportService(
        ReportRepository reportRepository,
        ReportDetailRepository reportDetailRepository,
        IReportCacheService cacheService,
        IKafkaProducerService kafkaProducerService)
    {
        _reportRepository = reportRepository;
        _reportDetailRepository = reportDetailRepository;
        _cacheService = cacheService;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.GetAll(isTracking: false)
            .ProjectToType<ReportListResponse>()
            .ToListAsync();

        return new SuccessDataResult<IEnumerable<ReportListResponse>>(reports, reports.Count);
    }

    public async Task<SuccessDataResult<ReportSmartResponse>> GetReportByIdAsync(Guid id)
    {
        // 1. Önce Rapor bulunur
        var report = await _reportRepository.GetByIdAsync(id);

        if (report == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor bulunamadı");

        // Raporun durumu kontrol edilir
        if (report.Status != ReportStatus.Completed)
        {
            var message = report.Status switch
            {
                ReportStatus.Preparing => "Raporun hazırlanması devam ediyor.",
                ReportStatus.Failed => "Rapor alımı sırasında hata oluştu. Raporu tekrar alınız.",
                _ => "Rapor durumu bilinmiyor."
            };

            return new SuccessDataResult<ReportSmartResponse>(new ReportSmartResponse
            {
                ReportId = report.Id,
                ReportType = report.Type,
                Status = report.Status,
                RequestedAt = report.RequestedAt,
                Parameters = report.Parameters, // Artık string
                Summary = report.Summary,
                Details = new List<ReportDetailResponse>(),
                DataSource = "null",
                Message = message
            });
        }

        // Rapor tamamlanmış ise detayları bulunur.
        // 2. Cache'den kontrol et
        var cachedData = await _cacheService.GetReportAsync(id);
        if (cachedData != null)
        {
            var result = new ReportSmartResponse
            {
                ReportId = report.Id,
                ReportType = report.Type,
                Status = report.Status,
                RequestedAt = report.RequestedAt,
                Parameters = report.Parameters,
                Summary = JsonConvert.DeserializeObject(report.Summary) ?? new(),
                Details = cachedData.Details.Select(d => new ReportDetailResponse
                {
                    Id = Guid.CreateVersion7(), // Cache'den gelen veri için geçici ID
                    Location = d.Location,
                    PersonCount = d.PersonCount,
                    PhoneCount = d.PhoneCount,
                    EmailCount = d.EmailCount
                }).ToList(),
                CreatedAt = cachedData.CreatedAt,
                ExpiresAt = cachedData.ExpiresAt,
                DataSource = "Cache (24 saat geçerli)",
                Message = "Rapor verisi cache'den alındı"
            };

            return new SuccessDataResult<ReportSmartResponse>(result);
        }

        // 3. Database'den ReportDetail kontrol et
        var reportWithDetails = await _reportRepository.GetWhere(x => x.Id == id, includeProperties: "ReportDetails", isTracking: false)
            .FirstOrDefaultAsync();

        if (reportWithDetails?.ReportDetails?.Any() == true)
        {
            var result = new ReportSmartResponse
            {
                ReportId = report.Id,
                ReportType = report.Type,
                Status = report.Status,
                RequestedAt = report.RequestedAt,
                Parameters = report.Parameters,
                Summary = JsonConvert.DeserializeObject(report.Summary) ?? new(),
                Details = reportWithDetails.ReportDetails.Select(d => new ReportDetailResponse
                {
                    Id = d.Id,
                    Location = d.Location,
                    PersonCount = d.PersonCount,
                    PhoneCount = d.PhoneCount,
                    EmailCount = d.EmailCount
                }).ToList(),
                DataSource = "Database (Kalıcı)",
                Message = "Rapor verisi kalıcı olarak kaydedilmiş"
            };

            return new SuccessDataResult<ReportSmartResponse>(result);
        }

        // 4. Sadece metadata döner
        var metadataResult = new ReportSmartResponse
        {
            ReportId = report.Id,
            ReportType = report.Type,
            Status = report.Status,
            RequestedAt = report.RequestedAt,
            Parameters = report.Parameters,
            Summary = JsonConvert.DeserializeObject(report.Summary) ?? new(),
            Details = new List<ReportDetailResponse>(),
            DataSource = "null",
            Message = "Rapor detayları bulunamadı. Cache süresi dolmuş veya kalıcı kaydedilmemiş."
        };

        return new SuccessDataResult<ReportSmartResponse>(metadataResult);
    }

    public async Task<SuccessResponse> CreateReportAsync(CreateReportRequest request)
    {
        var report = new Report
        {
            Status = ReportStatus.Preparing,
            Type = request.ReportType,
            Parameters = request.Parameters, // Artık string olduğu için direkt atanabilir
            RequestedAt = DateTime.UtcNow
        };

        await _reportRepository.AddAsync(report);
        await _reportRepository.SaveAsync();

        // Kafka'ya rapor işleme isteği gönder
        var kafkaMessage = new ReportRequestMessage
        {
            ReportId = report.Id,
            ReportType = request.ReportType,
            Parameters = report.Parameters,
            RequestedAt = report.RequestedAt
        };

        var kafkaResult = await _kafkaProducerService.SendReportRequestAsync(kafkaMessage);

        if (!kafkaResult)
        {
            // Kafka başarısız olursa hata fırlat
            throw new Setur.Contacts.Base.Exceptions.BusinessException("Rapor işleme sistemi şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.");
        }

        return new SuccessResponse(("1","Rapor başarıyla oluşturuldu ve işleme alındı"),report.Id.ToString());
    }

    public async Task<SuccessResponse> DeleteReportAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdAsync(id, throwException: false);
        if (report == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor bulunamadı");

        _reportRepository.Remove(report);
        await _reportRepository.SaveAsync();

        // Cache'den de sil
        await _cacheService.DeleteReportAsync(id);

        return new SuccessResponse("Rapor başarıyla silindi");
    }

    public async Task<SuccessResponse> SaveReportPermanentlyAsync(Guid reportId)
    {
        // Cache'den rapor verisini al
        var reportData = await _cacheService.GetReportAsync(reportId);
        if (reportData == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor verisi bulunamadı veya süresi dolmuş");

        // ReportDetail tablosuna kaydet
        var reportDetails = reportData.Details.Select(detail => new ReportDetail
        {
            ReportId = reportId,
            Location = detail.Location,
            PersonCount = detail.PersonCount,
            PhoneCount = detail.PhoneCount,
            EmailCount = detail.EmailCount
        }).ToList();

        await _reportDetailRepository.AddAsync(reportDetails);
        await _reportDetailRepository.SaveAsync();

        // Cache'den sil
        await _cacheService.DeleteReportAsync(reportId);

        return new SuccessResponse("Rapor kalıcı olarak kaydedildi");
    }

    public async Task<SuccessResponse> RetryReportAsync(Guid reportId)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        if (report == null)
            throw new Setur.Contacts.Base.Exceptions.NotFoundException("Rapor bulunamadı");

        if (report.Status != ReportStatus.Failed)
            throw new Setur.Contacts.Base.Exceptions.BusinessException("Sadece başarısız raporlar yeniden işlenebilir");

        // Rapor durumunu Preparing'e çevir
        report.Status = ReportStatus.Preparing;
        await _reportRepository.SaveAsync();

        // Kafka'ya yeniden işleme isteği gönder
        var kafkaMessage = new ReportRequestMessage
        {
            ReportId = report.Id,
            ReportType = report.Type,
            Parameters = report.Parameters,
            RequestedAt = report.RequestedAt
        };

        var kafkaResult = await _kafkaProducerService.SendReportRequestAsync(kafkaMessage);

        if (!kafkaResult)
        {
            // Kafka başarısız olursa raporu tekrar Failed yap
            report.Status = ReportStatus.Failed;
            await _reportRepository.SaveAsync();
            throw new Setur.Contacts.Base.Exceptions.BusinessException("Rapor işleme sistemi şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.");
        }

        return new SuccessResponse("Rapor yeniden işlemeye gönderildi");
    }
}
