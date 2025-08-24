using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.ReportApi.Services;

public interface IReportService
{
    /// <summary>
    /// Tüm raporları getirir
    /// </summary>
    /// <returns>Rapor listesi</returns>
    Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync();

    /// <summary>
    /// Belirtilen ID'ye sahip raporu detaylı bilgileri ile birlikte getirir.
    /// 
    /// Öncelik Sırası:
    /// 1. Cache'den kontrol eder (24 saat içinde oluşturulmuşsa)
    /// 2. Database'den ReportDetail tablosundan kontrol eder (kalıcı kaydedilmişse)
    /// 3. Sadece Report metadata'sını döner (detay yoksa)
    /// 
    /// </summary>
    /// <param name="id">Rapor ID'si</param>
    /// <returns>Akıllı rapor response modeli</returns>
    Task<SuccessDataResult<ReportSmartResponse>> GetReportByIdAsync(Guid id);

    /// <summary>
    /// Yeni bir rapor oluşturur
    /// </summary>
    /// <param name="request">Rapor oluşturma bilgileri</param>
    /// <returns>Oluşturma işlem sonucu</returns>
    Task<SuccessResponse> CreateReportAsync(CreateReportRequest request);

    /// <summary>
    /// Belirtilen ID'ye sahip raporu siler. Raporu bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">Rapor ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Silme işlem sonucu</returns>
    Task<SuccessResponse> DeleteReportAsync(Guid id);

    /// <summary>
    /// Raporu kalıcı olarak kaydeder (Cache'den ReportDetail tablosuna)
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <returns>Kaydetme işlem sonucu</returns>
    Task<SuccessResponse> SaveReportPermanentlyAsync(Guid reportId);

    /// <summary>
    /// Başarısız raporu yeniden hazırlanmaya gönderir
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <returns>Yeniden işleme sonucu</returns>
    Task<SuccessResponse> RetryReportAsync(Guid reportId);
}
