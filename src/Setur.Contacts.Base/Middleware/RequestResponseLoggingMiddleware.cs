using System.Text;
using Microsoft.AspNetCore.Http;
using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Middleware;

/// <summary>
/// HTTP isteklerini ve yanıtlarını loglayan middleware.<br/>
/// Proje genelinde eklenerek API uygulamasına gelen bütün istekleri ve yanıtları loglar.<br/>
/// Eğer hata oluşursa, hata mesajını da loglar.<br/>
/// Çalışması için <b>app.UseMiddleware()</b> şeklinde uygulamaya eklenmelidir.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Request logging
        var requestBody = await ReadRequestBodyAsync(context.Request);
        _logger.LogInformation(
            "HTTP {Method} {Path} isteği alındı. Query: {QueryString}, Body: {RequestBody}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            requestBody);

        // Response logging için orijinal response stream'i yedekle
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Response logging
            var response = await ReadResponseBodyAsync(context.Response);
            _logger.LogInformation(
                "HTTP {Method} {Path} yanıtı gönderildi. Status: {StatusCode}, Body: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                response);

            // Response'u orijinal stream'e kopyala
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"HTTP {context.Request.Method} {context.Request.Path} isteği işlenirken hata oluştu.",
                ex);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return text;
    }
} 