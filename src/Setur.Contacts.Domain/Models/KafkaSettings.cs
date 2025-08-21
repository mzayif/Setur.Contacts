namespace Setur.Contacts.Domain.Models;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
}
