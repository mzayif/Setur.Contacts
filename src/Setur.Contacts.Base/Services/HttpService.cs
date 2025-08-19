using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Models;

namespace Setur.Contacts.Base.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILoggerService _logger;
    private readonly Dictionary<string, HttpServiceConfig> _serviceConfigurations;
    private string _defaultService = string.Empty;

    public HttpService(HttpClient httpClient, ILoggerService logger, IOptions<HttpServiceOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serviceConfigurations = new Dictionary<string, HttpServiceConfig>();

        // Options Pattern ile konfigürasyonları yükle
        LoadConfigurations(options.Value);
    }

    #region Private Methods

    /// <summary>
    /// Options Pattern ile konfigürasyonları yükler
    /// </summary>
    private void LoadConfigurations(HttpServiceOptions options)
    {
        try
        {
            _logger.LogInformation("HTTP servis konfigürasyonları yükleniyor...");

            // Her servis konfigürasyonunu ekle
            foreach (var serviceConfig in options.Services)
            {
                _serviceConfigurations[serviceConfig.Key] = serviceConfig.Value;
                _logger.LogInformation($"Servis konfigürasyonu yüklendi: {serviceConfig.Key} - {serviceConfig.Value.BaseUrl}");
            }

            // Varsayılan servisi ayarla
            if (!string.IsNullOrEmpty(options.DefaultService))
            {
                _defaultService = options.DefaultService;
                _logger.LogInformation($"Varsayılan servis ayarlandı: {options.DefaultService}");
            }

            _logger.LogInformation($"HTTP servis konfigürasyonları başarıyla yüklendi. Toplam {options.Services.Count} servis.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex, "HTTP servis konfigürasyonları yüklenirken hata");
        }
    }

    /// <summary>
    /// REST servisler için yapılacak bütün HTTP isteklerini yöneten temel metod. <br/>
    /// HTTP istek türüne göre var ise request nesnesinde gelen veriyi body'e ekler, <br/>
    /// var ise Header'ları ekler, <br/>
    /// İstek sonucunda dönen bilgileri verilmiş ise Response tipine göre deserialize eder ve geriye HTTPResponse.Data içerisinde geri döndürür <br/>
    /// </summary>
    private async Task<HttpResponse<TResponse>> SendRequestAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest? request = null, string contentType = "application/json", Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            using var requestMessage = new HttpRequestMessage(method, url);

            // Content ekle
            if (request != null)
            {
                string contentString;
                if (contentType.Contains("json"))
                {
                    contentString = JsonSerializer.Serialize(request, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
                else if (contentType.Contains("xml"))
                {
                    contentString = SerializeToXml(request);
                }
                else
                {
                    contentString = request.ToString() ?? string.Empty;
                }

                requestMessage.Content = new StringContent(contentString, Encoding.UTF8, contentType);
            }

            // Header'ları ekle
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            // Timeout ayarla
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));

            var response = await _httpClient.SendAsync(requestMessage, cts.Token);
            stopwatch.Stop();

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = new HttpResponse<TResponse>
            {
                StatusCode = (int)response.StatusCode,
                RawContent = responseContent,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };

            // Response header'larını ekle
            foreach (var header in response.Headers)
            {
                result.Headers[header.Key] = string.Join(", ", header.Value);
            }

            // Yanıtı deserialize et
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    if (contentType.Contains("json"))
                    {
                        result.Data = JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    else if (contentType.Contains("xml"))
                    {
                        result.Data = DeserializeFromXml<TResponse>(responseContent);
                    }
                }
                catch (Exception ex)
                {
                    result.HasDeserializationError = true;
                    result.DeserializationErrorMessage = ex.Message;
                    _logger.LogWarning($"Yanıt deserializasyon hatası: {ex.Message}");
                }
            }

            _logger.LogInformation($"HTTP {method} isteği tamamlandı: {url}, Durum: {response.StatusCode}, Süre: {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"HTTP {method} isteği hatası: {url}, Hata: {ex.Message}");
            return new HttpResponse<TResponse>
            {
                StatusCode = 0,
                RawContent = string.Empty,
                ErrorMessage = ex.Message,
                ElapsedMilliseconds = 0
            };
        }
    }

    /// <summary>
    /// SOAP servisler için yapılacak bütün HTTP isteklerini yöneten temel metod. <br/>
    /// HTTP istek türüne göre var ise request nesnesinde gelen veriyi body'e ekler, <br/>
    /// var ise Header'ları ekler, <br/>
    /// İstek sonucunda dönen bilgileri verilmiş ise Response tipine göre deserialize eder ve geriye HTTPResponse.Data içerisinde geri döndürür <br/>
    /// </summary>
    private async Task<HttpResponse<TResponse>> SendSoapRequestInternalAsync<TRequest, TResponse>(string url, string soapAction, TRequest request, Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Request'i XML'e dönüştür
            string soapBody;
            if (request is string stringRequest)
            {
                soapBody = stringRequest;
            }
            else
            {
                soapBody = SerializeToXml(request);
            }

            // SOAP envelope oluştur
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Header/>
  <soap:Body>
    {soapBody}
  </soap:Body>
</soap:Envelope>";

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            // SOAP Action header'ı ekle
            requestMessage.Headers.Add("SOAPAction", soapAction);

            // Diğer header'ları ekle
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            // Timeout ayarla
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));

            var response = await _httpClient.SendAsync(requestMessage, cts.Token);
            stopwatch.Stop();

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = new HttpResponse<TResponse>
            {
                StatusCode = (int)response.StatusCode,
                RawContent = responseContent,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            };

            // Response header'larını ekle
            foreach (var header in response.Headers)
            {
                result.Headers[header.Key] = string.Join(", ", header.Value);
            }

            // Yanıtı deserialize et
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    result.Data = DeserializeFromXml<TResponse>(responseContent);
                }
                catch (Exception ex)
                {
                    result.HasDeserializationError = true;
                    result.DeserializationErrorMessage = ex.Message;
                    _logger.LogWarning($"SOAP yanıt deserializasyon hatası: {ex.Message}");
                }
            }

            _logger.LogInformation($"SOAP isteği tamamlandı: {url}, Durum: {response.StatusCode}, Süre: {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"SOAP isteği hatası: {url}, Hata: {ex.Message}");
            return new HttpResponse<TResponse>
            {
                StatusCode = 0,
                RawContent = string.Empty,
                ErrorMessage = ex.Message,
                ElapsedMilliseconds = 0
            };
        }
    }

    /// <summary>
    /// Configürasyon dosyasında tanımlanan base URL ile endpoint'i birleştirerek tam servis adresini geri döner
    /// </summary>
    private string CombineUrl(string baseUrl, string endpoint)
    {
        if (string.IsNullOrEmpty(baseUrl))
            return endpoint;

        if (string.IsNullOrEmpty(endpoint))
            return baseUrl;

        baseUrl = baseUrl.TrimEnd('/');
        endpoint = endpoint.TrimStart('/');

        return $"{baseUrl}/{endpoint}";
    }

    /// <summary>
    /// Configürasyon dosyasında tanımlanan sabit header'lar ile ek header'ları birleştirir. <br/>
    /// </summary>
    private Dictionary<string, string> MergeHeaders(HttpServiceConfig config, Dictionary<string, string>? additionalHeaders)
    {
        var mergedHeaders = new Dictionary<string, string>(config.DefaultHeaders);

        // Authentication header'larını ekle
        if (!string.IsNullOrEmpty(config.AuthToken))
        {
            mergedHeaders["Authorization"] = $"Bearer {config.AuthToken}";
        }

        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            mergedHeaders["X-API-Key"] = config.ApiKey;
        }

        if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Username}:{config.Password}"));
            mergedHeaders["Authorization"] = $"Basic {credentials}";
        }

        // Ek header'ları ekle
        if (additionalHeaders != null)
        {
            foreach (var header in additionalHeaders)
            {
                mergedHeaders[header.Key] = header.Value;
            }
        }

        return mergedHeaders;
    }

    /// <summary>
    /// SOAP servislerinde kullanılmak üzere gelen request nesnesini XML formatına serialize eder
    /// <para><b>Not:</b> Eğer request nesnesi string ise, olduğu gibi geri döner</para>
    /// </summary>
    private string SerializeToXml<T>(T obj)
    {
        if (obj is string stringObj)
            return stringObj;

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }

    /// <summary>
    /// SOAP servislerinde dönen XML yanıtını verilen TResponse tipine deserialize eder
    /// </summary>
    private T? DeserializeFromXml<T>(string xml) where T : class
    {
        if (typeof(T) == typeof(string))
            return xml as T;

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var stringReader = new StringReader(xml);
        return serializer.Deserialize(stringReader) as T;
    }

    #endregion

    #region Temel Metodlar

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, int timeout = 30) where TResponse : class
    {
        return await SendRequestAsync<object, TResponse>(HttpMethod.Get, url, null, "application/json", headers, timeout);
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, string contentType = "application/json", Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class
    {
        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Post, url, request, contentType, headers, timeout);
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest request, string contentType = "application/json", Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class
    {
        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Put, url, request, contentType, headers, timeout);
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null, int timeout = 30) where TResponse : class
    {
        return await SendRequestAsync<object, TResponse>(HttpMethod.Delete, url, null, "application/json", headers, timeout);
    }

    public async Task<HttpResponse<TResponse>> SendSoapRequestAsync<TRequest, TResponse>(string url, string soapAction, TRequest request, Dictionary<string, string>? headers = null, int timeout = 30) 
        where TRequest : class 
        where TResponse : class
    {
        return await SendSoapRequestInternalAsync<TRequest, TResponse>(url, soapAction, request, headers, timeout);
    }

    #endregion

    #region Konfigürasyonlu Metodlar

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string serviceName, string endpoint, Dictionary<string, string>? headers = null, int? timeout = null) where TResponse : class
    {
        var config = GetServiceConfiguration(serviceName);
        if (config == null)
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        var url = CombineUrl(config.BaseUrl, endpoint);
        var finalHeaders = MergeHeaders(config, headers);
        var finalTimeout = timeout ?? config.DefaultTimeout;

        return await SendRequestAsync<object, TResponse>(HttpMethod.Get, url, null, config.DefaultContentType, finalHeaders, finalTimeout);
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, string? contentType = null, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class
    {
        var config = GetServiceConfiguration(serviceName);
        if (config == null)
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        var url = CombineUrl(config.BaseUrl, endpoint);
        var finalHeaders = MergeHeaders(config, headers);
        var finalContentType = contentType ?? config.DefaultContentType;
        var finalTimeout = timeout ?? config.DefaultTimeout;

        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Post, url, request, finalContentType, finalHeaders, finalTimeout);
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, string? contentType = null, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class
    {
        var config = GetServiceConfiguration(serviceName);
        if (config == null)
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        var url = CombineUrl(config.BaseUrl, endpoint);
        var finalHeaders = MergeHeaders(config, headers);
        var finalContentType = contentType ?? config.DefaultContentType;
        var finalTimeout = timeout ?? config.DefaultTimeout;

        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Put, url, request, finalContentType, finalHeaders, finalTimeout);
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string serviceName, string endpoint, Dictionary<string, string>? headers = null, int? timeout = null) where TResponse : class
    {
        var config = GetServiceConfiguration(serviceName);
        if (config == null)
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        var url = CombineUrl(config.BaseUrl, endpoint);
        var finalHeaders = MergeHeaders(config, headers);
        var finalTimeout = timeout ?? config.DefaultTimeout;

        return await SendRequestAsync<object, TResponse>(HttpMethod.Delete, url, null, config.DefaultContentType, finalHeaders, finalTimeout);
    }

    public async Task<HttpResponse<TResponse>> SendSoapRequestAsync<TRequest, TResponse>(string serviceName, string endpoint, string soapAction, TRequest request, Dictionary<string, string>? headers = null, int? timeout = null) 
        where TRequest : class 
        where TResponse : class
    {
        var config = GetServiceConfiguration(serviceName);
        if (config == null)
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        var url = CombineUrl(config.BaseUrl, endpoint);
        var finalHeaders = MergeHeaders(config, headers);
        var finalTimeout = timeout ?? config.DefaultTimeout;

        return await SendSoapRequestInternalAsync<TRequest, TResponse>(url, soapAction, request, finalHeaders, finalTimeout);
    }

    #endregion

    #region Konfigürasyon Yönetimi

    public void AddServiceConfiguration(string serviceName, HttpServiceConfig config)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Servis adı boş olamaz");

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        config.ServiceName = serviceName;
        _serviceConfigurations[serviceName] = config;
        _logger.LogInformation($"Servis konfigürasyonu eklendi: {serviceName}");
    }

    public void UpdateServiceConfiguration(string serviceName, HttpServiceConfig config)
    {
        if (!_serviceConfigurations.ContainsKey(serviceName))
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        config.ServiceName = serviceName;
        _serviceConfigurations[serviceName] = config;
        _logger.LogInformation($"Servis konfigürasyonu güncellendi: {serviceName}");
    }

    public void RemoveServiceConfiguration(string serviceName)
    {
        if (_serviceConfigurations.Remove(serviceName))
        {
            _logger.LogInformation($"Servis konfigürasyonu kaldırıldı: {serviceName}");
        }
    }

    public HttpServiceConfig? GetServiceConfiguration(string serviceName)
    {
        return _serviceConfigurations.TryGetValue(serviceName, out var config) ? config : null;
    }

    public Dictionary<string, HttpServiceConfig> GetAllServiceConfigurations()
    {
        return new Dictionary<string, HttpServiceConfig>(_serviceConfigurations);
    }

    public void SetDefaultService(string serviceName)
    {
        if (!_serviceConfigurations.ContainsKey(serviceName))
            throw new ArgumentException($"Servis konfigürasyonu bulunamadı: {serviceName}");

        _defaultService = serviceName;
        _logger.LogInformation($"Varsayılan servis ayarlandı: {serviceName}");
    }

    public string GetDefaultService()
    {
        return _defaultService;
    }

    #endregion

} 