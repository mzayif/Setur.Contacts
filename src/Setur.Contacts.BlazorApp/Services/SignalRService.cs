using Microsoft.AspNetCore.SignalR.Client;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.BlazorApp.Services;

/// <summary>
/// SignalR bağlantısını yöneten ve rapor durumu güncellemelerini dinleyen servis
/// </summary>
public class SignalRService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<SignalRService> _logger;
    private readonly string _reportApiBaseUrl;
    private bool _isInitialized = false;

    public event Action<Guid, ReportStatus, string>? ReportStatusUpdated;

    public SignalRService(ILogger<SignalRService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _reportApiBaseUrl = configuration["SignalR:ReportHubUrl"] ?? 
                           configuration["ReportApiBaseUrl"] ?? 
                           "http://localhost:5002";
    }

    /// <summary>
    /// SignalR bağlantısını başlatır
    /// </summary>
    public async Task StartAsync()
    {
        try
        {
            // Eğer zaten başlatılmışsa, tekrar başlatma
            if (_isInitialized)
            {
                _logger.LogInformation("SignalR servisi zaten başlatılmış");
                return;
            }

            // Eğer bağlantı zaten varsa ve bağlıysa, tekrar başlatma
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                _logger.LogInformation("SignalR bağlantısı zaten aktif");
                return;
            }

            var hubUrl = $"{_reportApiBaseUrl}/reportHub";
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            // Rapor durumu güncellemelerini dinle
            _hubConnection.On<Guid, ReportStatus, string>("ReportStatusUpdated", OnReportStatusUpdated);

            // Bağlantı durumu değişikliklerini dinle
            _hubConnection.Closed += OnConnectionClosed;
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;

            await _hubConnection.StartAsync();
            _isInitialized = true;
            _logger.LogInformation("SignalR bağlantısı başlatıldı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignalR bağlantısı başlatılamadı");
        }
    }

    /// <summary>
    /// Belirli bir rapor grubuna katılır
    /// </summary>
    public async Task JoinReportGroupAsync(Guid reportId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinReportGroup", reportId);
        }
    }

    /// <summary>
    /// Belirli bir rapor grubundan ayrılır
    /// </summary>
    public async Task LeaveReportGroupAsync(Guid reportId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("LeaveReportGroup", reportId);
        }
    }

    private void OnReportStatusUpdated(Guid reportId, ReportStatus status, string message)
    {
        ReportStatusUpdated?.Invoke(reportId, status, message);
    }

    private Task OnConnectionClosed(Exception? exception)
    {
        _logger.LogWarning(exception, "SignalR bağlantısı kapandı");
        return Task.CompletedTask;
    }

    private Task OnReconnecting(Exception? exception)
    {
        _logger.LogInformation(exception, "SignalR yeniden bağlanıyor...");
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation($"SignalR yeniden bağlandı. ConnectionId: {connectionId}");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
