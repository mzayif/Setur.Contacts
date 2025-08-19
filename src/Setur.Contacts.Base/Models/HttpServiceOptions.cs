namespace Setur.Contacts.Base.Models;

/// <summary>
/// HTTP servis konfigürasyonları için Options Pattern modeli
/// </summary>
public class HttpServiceOptions
{
    /// <summary>
    /// Varsayılan servis adı
    /// </summary>
    public string DefaultService { get; set; } = string.Empty;

    /// <summary>
    /// Servis konfigürasyonları
    /// </summary>
    public Dictionary<string, HttpServiceConfig> Services { get; set; } = new();
} 