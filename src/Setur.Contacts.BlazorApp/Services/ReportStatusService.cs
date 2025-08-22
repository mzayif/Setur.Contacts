using Setur.Contacts.BlazorApp.Services.Abstracts;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Responses;

namespace Setur.Contacts.BlazorApp.Services;

public class ReportStatusService : IReportStatusService
{
    private readonly IReportService _reportService;
    private readonly Dictionary<Guid, Timer> _monitoringTimers = new();
    private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokens = new();

    public event Action<ReportResponse>? ReportCompleted;
    public event Action<string>? ReportFailed;

    public ReportStatusService(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task StartMonitoringReport(Guid reportId)
    {
        if (_monitoringTimers.ContainsKey(reportId))
            return Task.CompletedTask;

        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokens[reportId] = cancellationTokenSource;

        var timer = new Timer(async _ => await CheckReportStatus(reportId, cancellationTokenSource.Token), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        _monitoringTimers[reportId] = timer;
        
        return Task.CompletedTask;
    }

    public Task StopMonitoringReport(Guid reportId)
    {
        if (_monitoringTimers.TryGetValue(reportId, out var timer))
        {
            timer.Dispose();
            _monitoringTimers.Remove(reportId);
        }

        if (_cancellationTokens.TryGetValue(reportId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            _cancellationTokens.Remove(reportId);
        }

        return Task.CompletedTask;
    }

    public async Task<bool> IsReportCompleted(Guid reportId)
    {
        try
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            return report?.Data?.Status == ReportStatus.Completed;
        }
        catch
        {
            return false;
        }
    }

    private async Task CheckReportStatus(Guid reportId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        try
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            
            if (report?.Data == null)
                return;

            switch (report.Data.Status)
            {
                case ReportStatus.Completed:
                    ReportCompleted?.Invoke(report.Data);
                    await StopMonitoringReport(reportId);
                    break;
                    
                case ReportStatus.Failed:
                    ReportFailed?.Invoke($"Rapor oluşturulamadı: Bilinmeyen hata");
                    await StopMonitoringReport(reportId);
                    break;
                    
                case ReportStatus.Preparing:
                    // Devam et, henüz tamamlanmadı
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportFailed?.Invoke($"Rapor durumu kontrol edilirken hata oluştu: {ex.Message}");
            await StopMonitoringReport(reportId);
        }
    }
}
