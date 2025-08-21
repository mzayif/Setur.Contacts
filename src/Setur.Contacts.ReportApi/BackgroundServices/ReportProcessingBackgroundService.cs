using Setur.Contacts.Domain.Enums;
using Setur.Contacts.ReportApi.Services;

namespace Setur.Contacts.ReportApi.BackgroundServices;

/// <summary>
/// <b>Report Processing Background Service</b><br/>
///  <br/>
/// Bu servis, rapor işleme isteklerini kuyrukta bekletir ve sırayla işler. <br/>
///  <br/>
/// İş Akışı: <br/>
/// 1. ReportService'den gelen rapor istekleri kuyruğa eklenir <br/>
/// 2. Background service sürekli kuyruğu kontrol eder <br/>
/// 3. Kuyruktan istek alır ve ReportProcessorService'e gönderir <br/>
/// 4. İşlem tamamlandığında bir sonraki isteği alır <br/>
///   <br/>
/// Tetiklenme: <br/>
/// - ReportService.CreateReportAsync() tarafından tetiklenir <br/>
/// - Uygulama başladığında otomatik olarak çalışır <br/>
///  <br/>
/// Gelecek Geliştirme: <br/>
/// - Kafka ile değiştirilecek <br/>
/// - Mikroservisler arası iletişim için <br/>
/// </summary>
public class ReportProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReportProcessingBackgroundService> _logger;
    private readonly Queue<ReportRequest> _reportQueue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public ReportProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ReportProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Report Processing Background Service başlatıldı");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ReportRequest? reportRequest = null;

                await _semaphore.WaitAsync(stoppingToken);
                try
                {
                    if (_reportQueue.Count > 0)
                    {
                        reportRequest = _reportQueue.Dequeue();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }

                if (reportRequest != null)
                {
                    await ProcessReportAsync(reportRequest);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background service'de hata oluştu");
                await Task.Delay(5000, stoppingToken);
            }
        }

        _logger.LogInformation("Report Processing Background Service durduruldu");
    }

    private async Task ProcessReportAsync(ReportRequest reportRequest)
    {
        using var scope = _serviceProvider.CreateScope();
        var reportProcessor = scope.ServiceProvider.GetRequiredService<IReportProcessorService>();

        await reportProcessor.ProcessReportAsync(
            reportRequest.ReportId,
            reportRequest.ReportType,
            reportRequest.Parameters);
    }

    /// <summary>
    /// Rapor işleme isteğini kuyruğa ekler
    /// </summary>
    /// <param name="reportRequest">İşlenecek rapor isteği</param>
    /// <returns>Kuyruğa ekleme işlemi</returns>
    public async Task EnqueueReportAsync(ReportRequest reportRequest)
    {
        await _semaphore.WaitAsync();
        try
        {
            _reportQueue.Enqueue(reportRequest);
            _logger.LogInformation($"Rapor kuyruğa eklendi. ReportId: {reportRequest.ReportId}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public class ReportRequest
    {
        public Guid ReportId { get; set; }
        public ReportType ReportType { get; set; }
        public string Parameters { get; set; } = string.Empty;
    }
}
