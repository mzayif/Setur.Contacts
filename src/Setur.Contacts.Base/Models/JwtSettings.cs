namespace Setur.Contacts.Base.Models;

/// <summary>
/// JWT ayarları için model sınıfı
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// JWT secret key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token geçerlilik süresi (dakika)
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token geçerlilik süresi (gün)
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
} 