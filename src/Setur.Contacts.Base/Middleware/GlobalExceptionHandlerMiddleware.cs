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
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
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
                result = new ErrorResponse("BUSINES_ERROR", businessEx.Message);
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
}