using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Setur.Contacts.Domain.CommonModels;
using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

/// <summary>
/// <b>Kafka Consumer Service</b><br/>
/// <br/>
/// Bu servis, Kafka'dan gelen rapor işleme mesajlarını dinler ve işler. Background service olarak çalışır.<br/>
/// <br/>
/// İş Akışı:<br/>
/// 1. Kafka topic'ine subscribe olur<br/>
/// 2. Gelen mesajları sürekli dinler<br/>
/// 3. Her mesajı JSON'dan deserialize eder<br/>
/// 4. ReportProcessorService'i çağırarak rapor işleme sürecini başlatır<br/>
/// 5. Mesaj işleme başarılı olduğunda commit eder<br/>
/// <br/>
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaAdminService _kafkaAdminService;

    public KafkaConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        KafkaAdminService kafkaAdminService)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _kafkaAdminService = kafkaAdminService;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    /// <summary>
    /// Background service'in ana çalışma metodu. Kafka'dan mesaj dinlemeye başlar.
    /// </summary>
    /// <param name="stoppingToken">Servisin durdurulması için kullanılan cancellation token</param>
    /// <returns>Task</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Topic'in var olduğundan emin ol
            await _kafkaAdminService.EnsureTopicExistsAsync();

            _consumer.Subscribe(_kafkaSettings.TopicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka Consumer Service başlatılırken hata oluştu");
            throw;
        }

        // Kafka consumer'ı ayrı bir task'te çalıştır
        _ = Task.Run(async () =>
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);
                        await ProcessMessageAsync(consumeResult.Message.Value);
                        _consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka mesaj tüketme hatası");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Beklenmeyen hata");
                    }
                }
            }
            finally
            {
                _consumer.Close();
            }
        }, stoppingToken);
    }

    /// <summary>
    /// Kafka'dan gelen JSON mesajını işler ve rapor işleme sürecini başlatır.
    /// </summary>
    /// <param name="messageJson">Kafka'dan gelen JSON formatındaki mesaj</param>
    /// <returns>Task</returns>
    private async Task ProcessMessageAsync(string messageJson)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<ReportRequestMessage>(messageJson);
            if (message == null)
            {
                _logger.LogWarning("Geçersiz mesaj formatı: {Message}", messageJson);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            // Setur.Contacts.ReportApi üst uygulama olduğu için ReportProcessorService'i dinamik olarak alınır
            var reportProcessorType = Type.GetType("Setur.Contacts.ReportApi.Services.IReportProcessorService, Setur.Contacts.ReportApi");
            if (reportProcessorType != null)
            {
                var reportProcessor = scope.ServiceProvider.GetService(reportProcessorType);
                if (reportProcessor != null)
                {
                    var processMethod = reportProcessorType.GetMethod("ProcessReportAsync");
                    if (processMethod != null)
                    {
                        //Raporu asıl işleyecek metodu dinamik olarak çalıştırır
                        await (Task)processMethod.Invoke(reportProcessor, new object[]
                        {
                            message.ReportId,
                            message.ReportType,
                            message.Parameters
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mesaj işleme hatası: {Message}", messageJson);
        }
    }

    public void StopConsuming()
    {
        _consumer?.Close();
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
