using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private const string TopicName = "report-requests";

    public KafkaProducerService(ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092", // Kafka server adresi
            ClientId = "setur-contacts-producer"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> SendReportRequestAsync(ReportRequestMessage message)
    {
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.ReportId.ToString(),
                Value = jsonMessage
            };

            var result = await _producer.ProduceAsync(TopicName, kafkaMessage);
            
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
