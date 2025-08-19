using Setur.Contacts.Base.Models;

namespace Setur.Contacts.Base.Interfaces;

/// <summary>
/// Uygulama içerisinden dış servislere yapılabilecek bütün REST ve SOAP isteklerini yönetmek için kullanılacak arayüz.
/// </summary>
public interface IHttpService
{
    /// <summary>
    /// GET isteği gönderir ve generic model döner
    /// </summary>
    Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, int timeout = 30) where TResponse : class;

    /// <summary>
    /// POST isteği gönderir ve generic model döner
    /// </summary>
    Task<HttpResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, string contentType = "application/json", Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// PUT isteği gönderir ve generic model döner
    /// </summary>
    Task<HttpResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest request, string contentType = "application/json", Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// DELETE isteği gönderir ve generic model döner
    /// </summary>
    Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null, int timeout = 30) where TResponse : class;

    /// <summary>
    /// SOAP isteği gönderir ve generic model döner
    /// </summary>
    Task<HttpResponse<TResponse>> SendSoapRequestAsync<TRequest, TResponse>(string url, string soapAction, TRequest request, Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class;

    #region Konfigürasyonlu Metodlar

    /// <summary>
    /// Belirli bir servis konfigürasyonu ile GET isteği gönderir
    /// </summary>
    Task<HttpResponse<TResponse>> GetAsync<TResponse>(string serviceName, string endpoint, Dictionary<string, string>? headers = null, int? timeout = null) where TResponse : class;

    /// <summary>
    /// Belirli bir servis konfigürasyonu ile POST isteği gönderir
    /// </summary>
    Task<HttpResponse<TResponse>> PostAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, string? contentType = null, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// Belirli bir servis konfigürasyonu ile PUT isteği gönderir
    /// </summary>
    Task<HttpResponse<TResponse>> PutAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, string? contentType = null, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class;

    /// <summary>
    /// Belirli bir servis konfigürasyonu ile DELETE isteği gönderir
    /// </summary>
    Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string serviceName, string endpoint, Dictionary<string, string>? headers = null, int? timeout = null) where TResponse : class;

    /// <summary>
    /// Belirli bir servis konfigürasyonu ile SOAP isteği gönderir
    /// </summary>
    Task<HttpResponse<TResponse>> SendSoapRequestAsync<TRequest, TResponse>(string serviceName, string endpoint, string soapAction, TRequest request, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class;

    #endregion

    #region Konfigürasyon Yönetimi

    /// <summary>
    /// Servis konfigürasyonu ekler
    /// </summary>
    void AddServiceConfiguration(string serviceName, HttpServiceConfig config);

    /// <summary>
    /// Servis konfigürasyonu günceller
    /// </summary>
    void UpdateServiceConfiguration(string serviceName, HttpServiceConfig config);

    /// <summary>
    /// Servis konfigürasyonu kaldırır
    /// </summary>
    void RemoveServiceConfiguration(string serviceName);

    /// <summary>
    /// Servis konfigürasyonu alır
    /// </summary>
    HttpServiceConfig? GetServiceConfiguration(string serviceName);

    /// <summary>
    /// Tüm servis konfigürasyonlarını alır
    /// </summary>
    Dictionary<string, HttpServiceConfig> GetAllServiceConfigurations();

    /// <summary>
    /// Varsayılan servis adını ayarlar
    /// </summary>
    void SetDefaultService(string serviceName);

    /// <summary>
    /// Varsayılan servis adını alır
    /// </summary>
    string GetDefaultService();

    #endregion
}

/// <summary>
/// Generic HTTP yanıt modeli
/// </summary>
public class HttpResponse<T> where T : class
{
    public int StatusCode { get; set; }
    public string RawContent { get; set; } = string.Empty;
    public T? Data { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;
    public string? ErrorMessage { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public bool HasDeserializationError { get; set; }
    public string? DeserializationErrorMessage { get; set; }
} 