using System.Text.Json.Serialization;

namespace Setur.Contacts.BlazorApp.Models;

/// <summary>
/// ASP.NET Core'un validation error response formatı
/// </summary>
public class ValidationErrorResponse
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
}
