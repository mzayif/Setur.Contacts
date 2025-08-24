using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services.Abstracts;

public interface IReportStatusService
{
    event Action<ReportSmartResponse> ReportCompleted;
    event Action<string> ReportFailed;

    Task StartMonitoringReport(Guid reportId);
    Task StopMonitoringReport(Guid reportId);
    Task<bool> IsReportCompleted(Guid reportId);
}
