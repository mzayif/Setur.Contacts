using System.Net;
using System.Text.Json;
using Setur.Contacts.Base.Results;
using Setur.Contacts.BlazorApp.Services.Abstracts;

namespace Setur.Contacts.BlazorApp.Services;

/// <summary>
/// HTTP yanıtlarını merkezi olarak handle eden ve hata durumlarında ErrorResponse modelini toast mesajı olarak gösteren Message Handler
/// </summary>
public class ErrorHandlingHttpMessageHandler : DelegatingHandler
{
    private readonly IToastService _toastService;

    public ErrorHandlingHttpMessageHandler(IToastService toastService)
    {
        _toastService = toastService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // 200 harici HTTP yanıtlarını kontrol et
        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response);
        }

        return response;
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        // Response body'sini oku
        var content = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrEmpty(content))
        {
            // ErrorResponse modelini deserialize etmeye çalış
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (errorResponse != null)
            {
                // ErrorResponse'dan mesajı al
                var errorMessage = GetErrorMessage(errorResponse);
                throw new HttpRequestException(errorMessage);
            }
        }

        // ErrorResponse yoksa veya deserialize edilemezse, HTTP status code'a göre genel mesaj göster
        var generalMessage = GetGeneralErrorMessage(response.StatusCode);
        throw new HttpRequestException(generalMessage);
    }

    private string GetErrorMessage(ErrorResponse errorResponse)
    {
        // ErrorMessages listesi varsa ilk mesajı al
        if (errorResponse.ErrorMessages?.Any() == true)
        {
            return errorResponse.ErrorMessages.First();
        }

        // Message property'si varsa onu al
        if (!string.IsNullOrEmpty(errorResponse.Message))
        {
            return errorResponse.Message;
        }

        // Hiçbiri yoksa genel mesaj
        return "Bir hata oluştu";
    }

    private string GetGeneralErrorMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Geçersiz istek",
            HttpStatusCode.Unauthorized => "Yetkisiz erişim",
            HttpStatusCode.Forbidden => "Erişim reddedildi",
            HttpStatusCode.NotFound => "Kaynak bulunamadı",
            HttpStatusCode.InternalServerError => "Sunucu hatası",
            HttpStatusCode.ServiceUnavailable => "Servis kullanılamıyor",
            _ => $"HTTP {((int)statusCode)} hatası"
        };
    }
}
