namespace Setur.Contacts.Base.Interfaces;

public interface IRefreshTokenService
{
    /// <summary>
    /// Yeni refresh token oluşturur
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Oluşturulan refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token'ı doğrular ve yeni access token oluşturur
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Yeni access token ve refresh token</returns>
    Task<(string AccessToken, string RefreshToken)?> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token'ı iptal eder
    /// </summary>
    /// <param name="refreshToken">İptal edilecek refresh token</param>
    /// <param name="reason">İptal nedeni</param>
    /// <param name="revokedBy">İptal eden</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    Task RevokeRefreshTokenAsync(string refreshToken, string reason, string revokedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcının tüm refresh token'larını iptal eder
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <param name="reason">İptal nedeni</param>
    /// <param name="revokedBy">İptal eden</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    Task RevokeAllUserTokensAsync(Guid userId, string reason, string revokedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token'ın aktif olup olmadığını kontrol eder
    /// </summary>
    /// <param name="refreshToken">Kontrol edilecek refresh token</param>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Token aktif mi?</returns>
    Task<bool> IsTokenActiveAsync(string refreshToken, CancellationToken cancellationToken = default);
} 