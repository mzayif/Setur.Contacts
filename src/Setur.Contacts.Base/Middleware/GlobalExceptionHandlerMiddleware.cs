using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Results;
using FluentValidation;

namespace Setur.Contacts.Base.Middleware;

/// <summary>
/// Bu Middleware, uygulama genelinde meydana gelen hatalar� yakalar ve <see cref="ErrorResponse"/> paternine uygun HTTP yan�tlar�n� d�ner.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Response body'yi yakalamak için stream'i değiştir
        var originalBodyStream = context.Response.Body;
        
        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);
            
            // next() çağrısından sonra HTTP status code'unu kontrol et
            if (context.Response.StatusCode != 200 && context.Response.StatusCode != 0)
            {
                await HandleNonSuccessResponseAsync(context, responseBody, originalBodyStream);
            }
            else
            {
                // Başarılı response'ları normal şekilde geri yaz
                context.Response.Body = originalBodyStream;
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        catch (Exception ex)
        {
            context.Response.Body = originalBodyStream;
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        ErrorResponse result;
        switch (exception)
        {
            case FluentValidation.ValidationException fluentValidationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new ErrorResponse("VALIDATION_ERROR", "Doğrulama hatası")
                {
                    ErrorMessages = fluentValidationEx.Errors.Select(e => e.ErrorMessage).ToList()
                };
                break;

            case Setur.Contacts.Base.Exceptions.ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new ErrorResponse("VALIDATION_ERROR", "Validation failed")
                {
                    ErrorMessages = validationEx.Errors.SelectMany(e => e.Value).ToList()
                };

                break;

            case BusinessException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new ErrorResponse(businessEx.ErrorCode, businessEx.Message);
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                result = new ErrorResponse("NOT_FOUND_ERROR", notFoundEx.EntityName);
                break;

            case UnauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                result = new ErrorResponse("UNAUTHORIZED_ERROR", "Login Error");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                result = new ErrorResponse("GENERAL_ERROR", exception.Message);
                break;
        }

        _logger.LogError("Global exception handler caught an exception", exception);

        var jsonResult = JsonSerializer.Serialize(result);
        await response.WriteAsync(jsonResult);
    }

    private async Task HandleNonSuccessResponseAsync(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
    {
        var response = context.Response;
        
        // Response body'yi oku
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
        
        // Sadece 400 Bad Request'leri ErrorResponse'a dönüştür
        if (response.StatusCode == 400 && !string.IsNullOrEmpty(responseBodyText))
        {
            try
            {
                // ASP.NET Core validation error formatını kontrol et
                using var jsonDoc = JsonDocument.Parse(responseBodyText);
                var root = jsonDoc.RootElement;
                
                if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    // Validation hatalarını topla
                    var errorMessages = new List<string>();
                    foreach (var error in errorsElement.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var errorMessage in error.Value.EnumerateArray())
                            {
                                if (errorMessage.ValueKind == JsonValueKind.String)
                                {
                                    errorMessages.Add(errorMessage.GetString()!);
                                }
                            }
                        }
                    }
                    
                    if (errorMessages.Any())
                    {
                        // ErrorResponse formatında yeni response oluştur
                        var errorResponse = new ErrorResponse("VALIDATION_ERROR", "Doğrulama hatası")
                        {
                            ErrorMessages = errorMessages
                        };
                        
                        var jsonResult = JsonSerializer.Serialize(errorResponse);
                        
                        // Response'u sıfırla ve yeni content'i yaz
                        response.Body = originalBodyStream;
                        response.ContentType = "application/json";
                        response.ContentLength = null;
                        
                        await response.WriteAsync(jsonResult);
                        return;
                    }
                }
            }
            catch (JsonException)
            {
                // JSON parse hatası durumunda genel mesaj göster
            }
        }
        
        // Eğer validation error değilse, orijinal response'u geri yaz
        response.Body = originalBodyStream;
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}