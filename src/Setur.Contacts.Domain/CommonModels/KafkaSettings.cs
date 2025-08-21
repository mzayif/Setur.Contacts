namespace Setur.Contacts.Domain.CommonModels
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
    }
}
