namespace Setur.Contacts.Base.Models
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        public string[] AllowedHeaders { get; set; } = new[] { "*" };
        public bool AllowCredentials { get; set; } = true;
        public int MaxAge { get; set; } = 86400; // 24 saat
    }
} 