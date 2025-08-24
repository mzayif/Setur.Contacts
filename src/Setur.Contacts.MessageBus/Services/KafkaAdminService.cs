using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Setur.Contacts.Domain.CommonModels;

namespace Setur.Contacts.MessageBus.Services;

/// <summary>
/// Kafkada Topic yönetimi için kullanılacak servis. Topic oluşturma, silme ve listeleme işlemlerini yapar.
/// </summary>
public class KafkaAdminService
{
    private readonly ILogger<KafkaAdminService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaAdminService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaAdminService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task EnsureTopicExistsAsync()
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            // Topic'in var olup olmadığını kontrol et
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topicExists = metadata.Topics.Any(t => t.Topic == _kafkaSettings.TopicName);

            if (!topicExists)
            {
                var topicSpec = new TopicSpecification
                {
                    Name = _kafkaSettings.TopicName,
                    ReplicationFactor = 1, // Development için 1, production için 3
                    NumPartitions = 3 // 3 partition
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpec });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka topic oluşturma hatası: {TopicName}", _kafkaSettings.TopicName);
            throw;
        }
    }

    public async Task DeleteTopicAsync(string topicName)
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            await adminClient.DeleteTopicsAsync(new[] { topicName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka topic silme hatası: {TopicName}", topicName);
            throw;
        }
    }

    public async Task<List<string>> ListTopicsAsync()
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            return metadata.Topics.Select(t => t.Topic).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka topic listesi alma hatası");
            throw;
        }
    }
}
