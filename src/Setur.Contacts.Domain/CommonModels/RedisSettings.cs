namespace Setur.Contacts.Domain.CommonModels
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public int DefaultDatabase { get; set; } = 0;
    }
}
