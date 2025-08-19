namespace Setur.Contacts.Base.Models;

/// <summary>
/// HTTP servis konfigürasyonu
/// </summary>
public class HttpServiceConfig
{
    /// <summary>
    /// Servis adı (benzersiz tanımlayıcı)
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Base URL
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Varsayılan timeout süresi (saniye)
    /// </summary>
    public int DefaultTimeout { get; set; } = 30;

    /// <summary>
    /// Varsayılan Content-Type
    /// </summary>
    public string DefaultContentType { get; set; } = "application/json";

    /// <summary>
    /// Sabit header'lar
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();

    /// <summary>
    /// Authentication token
    /// </summary>
    public string? AuthToken { get; set; }

    /// <summary>
    /// API Key
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Kullanıcı adı
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Şifre
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Retry sayısı
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Retry delay (milisaniye)
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// SSL sertifika doğrulamasını atla
    /// </summary>
    public bool SkipSslValidation { get; set; } = false;

    /// <summary>
    /// Proxy kullanımı
    /// </summary>
    public bool UseProxy { get; set; } = false;

    /// <summary>
    /// Proxy URL
    /// </summary>
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// Proxy kullanıcı adı
    /// </summary>
    public string? ProxyUsername { get; set; }

    /// <summary>
    /// Proxy şifre
    /// </summary>
    public string? ProxyPassword { get; set; }
}

/// <summary>
/// HTTP servis konfigürasyonları koleksiyonu
/// </summary>
public class HttpServiceConfigurations
{
    /// <summary>
    /// Servis konfigürasyonları
    /// </summary>
    public Dictionary<string, HttpServiceConfig> Services { get; set; } = new();

    /// <summary>
    /// Varsayılan servis adı
    /// </summary>
    public string DefaultService { get; set; } = string.Empty;
} 