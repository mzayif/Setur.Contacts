using Setur.Contacts.Base.Results;
using Setur.Contacts.Domain.Requests;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services.Abstracts;

public interface IReportService
{
    Task<SuccessResponse> CreateReportAsync(CreateReportRequest request);
    Task<SuccessDataResult<ReportSmartResponse?>> GetReportByIdAsync(Guid id);
    Task<SuccessDataResult<IEnumerable<ReportListResponse>>> GetAllReportsAsync();
    Task<SuccessResponse> DeleteReportAsync(Guid id);
    Task<SuccessDataResult<ReportDetailResponse?>> GetReportDetailsAsync(Guid id);
    Task<SuccessResponse> RetryReportAsync(Guid id);
    Task<SuccessResponse> SaveReportPermanentlyAsync(Guid id);
}
