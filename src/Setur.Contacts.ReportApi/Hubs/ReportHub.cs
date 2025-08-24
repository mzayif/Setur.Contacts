using Microsoft.AspNetCore.SignalR;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.ReportApi.Hubs;

/// <summary>
/// Rapor durumu değişikliklerini client'lara bildiren SignalR Hub
/// </summary>
public class ReportHub : Hub
{
    private readonly ILoggerService _loggerService;

    public ReportHub(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    /// <summary>
    /// Rapor durumu güncellendiğinde tüm client'lara bildirim gönderir
    /// </summary>
    public async Task ReportStatusUpdated(Guid reportId, ReportStatus status, string message = "")
    {
        await Clients.All.SendAsync("ReportStatusUpdated", reportId, status, message);
    }

    /// <summary>
    /// Belirli bir raporu takip eden client'lara bildirim gönderir
    /// </summary>
    public async Task ReportStatusUpdatedForReport(Guid reportId, ReportStatus status, string message = "")
    {
        await Clients.Group($"Report_{reportId}").SendAsync("ReportStatusUpdated", reportId, status, message);
    }

    /// <summary>
    /// Client'ı belirli bir rapor grubuna ekler
    /// </summary>
    public async Task JoinReportGroup(Guid reportId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Report_{reportId}");
    }

    /// <summary>
    /// Client'ı belirli bir rapor grubundan çıkarır
    /// </summary>
    public async Task LeaveReportGroup(Guid reportId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Report_{reportId}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _loggerService.LogInformation($"SignalR bağlantısı kuruldu. ConnectionId: {Context.ConnectionId}, Toplam bağlantı: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        _loggerService.LogInformation($"SignalR bağlantısı kesildi. ConnectionId: {Context.ConnectionId}");
    }
}
