using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ReportApi.DTOs.Requests;
using Setur.Contacts.ReportApi.DTOs.Responses;

namespace Setur.Contacts.ReportApi.Services;

public interface IReportService
{
    /// <summary>
    /// Tüm raporları getirir
    /// </summary>
    /// <returns>Rapor listesi</returns>
    Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync();

    /// <summary>
    /// Belirtilen ID'ye sahip raporu detaylarıyla birlikte getirir. Raporu bulamazsa NotFoundException fırlatır.
    /// </summary>
    /// <param name="id">Rapor ID'si</param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns>Rapor detayları</returns>
    Task<SuccessDataResult<ReportResponse?>> GetReportByIdAsync(Guid id);

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
}
