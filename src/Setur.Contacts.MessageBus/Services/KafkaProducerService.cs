using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Setur.Contacts.Domain.CommonModels;
using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly KafkaAdminService _kafkaAdminService;

    public KafkaProducerService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaProducerService> logger,
        KafkaAdminService kafkaAdminService)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _kafkaAdminService = kafkaAdminService;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            ClientId = _kafkaSettings.ClientId
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> SendReportRequestAsync(ReportRequestMessage message)
    {
        try
        {
            // Topic'in var olduğundan emin ol
            await _kafkaAdminService.EnsureTopicExistsAsync();

            var jsonMessage = JsonConvert.SerializeObject(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.ReportId.ToString(),
                Value = jsonMessage
            };

            var result = await _producer.ProduceAsync(_kafkaSettings.TopicName, kafkaMessage);

            _logger.LogInformation("Report request sent to Kafka. ReportId: {ReportId}, Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                message.ReportId, result.Topic, result.Partition, result.Offset);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending report request to Kafka. ReportId: {ReportId}", message.ReportId);
            return false;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
