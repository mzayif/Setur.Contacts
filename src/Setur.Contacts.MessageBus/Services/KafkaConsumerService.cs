using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Setur.Contacts.Domain.CommonModels;
using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceProvider _serviceProvider;

    public KafkaConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service başlatıldı. Topic: {Topic}", _kafkaSettings.TopicName);

        _consumer.Subscribe(_kafkaSettings.TopicName);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    _logger.LogInformation("Kafka'dan mesaj alındı. Key: {Key}, Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                        consumeResult.Message.Key, consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

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

        _logger.LogInformation("Kafka Consumer Service durduruldu");
    }

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
            // ReportProcessorService'i dinamik olarak al
            var reportProcessorType = Type.GetType("Setur.Contacts.ReportApi.Services.IReportProcessorService, Setur.Contacts.ReportApi");
            if (reportProcessorType != null)
            {
                var reportProcessor = scope.ServiceProvider.GetService(reportProcessorType);
                if (reportProcessor != null)
                {
                    var processMethod = reportProcessorType.GetMethod("ProcessReportAsync");
                    if (processMethod != null)
                    {
                        await (Task)processMethod.Invoke(reportProcessor, new object[]
                        {
                            message.ReportId,
                            message.ReportType,
                            message.Parameters
                        });
                    }
                }
            }

            _logger.LogInformation("Mesaj başarıyla işlendi. ReportId: {ReportId}", message.ReportId);
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
