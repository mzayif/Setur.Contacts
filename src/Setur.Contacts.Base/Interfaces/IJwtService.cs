namespace Setur.Contacts.Base.Interfaces;

/// <summary>
/// JWT işlemleri için service interface'i
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Kullanıcı bilgilerine göre JWT token oluşturur
    /// </summary>
    /// <param name="user">Kullanıcı bilgileri</param>
    /// <returns>JWT token</returns>
    string GenerateToken(object user);

    /// <summary>
    /// Refresh token oluşturur
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <returns>Refresh token</returns>
    string GenerateRefreshToken(Guid userId);

    /// <summary>
    /// JWT token'ı doğrular
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Token geçerli mi?</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// JWT token'dan kullanıcı ID'sini çıkarır
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Kullanıcı ID</returns>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// JWT token'dan kullanıcı email'ini çıkarır
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Kullanıcı email</returns>
    string? GetUserEmailFromToken(string token);
} 