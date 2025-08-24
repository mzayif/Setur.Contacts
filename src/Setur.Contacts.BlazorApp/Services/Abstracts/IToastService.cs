namespace Setur.Contacts.BlazorApp.Services.Abstracts;

/// <summary>
/// Toast notification servisi için interface
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Başarı mesajı gösterir
    /// </summary>
    /// <param name="message">Gösterilecek mesaj</param>
    /// <param name="title">Mesaj başlığı (opsiyonel)</param>
    void ShowSuccess(string message, string? title = null);

    /// <summary>
    /// Hata mesajı gösterir
    /// </summary>
    /// <param name="message">Gösterilecek hata mesajı</param>
    /// <param name="title">Mesaj başlığı (opsiyonel)</param>
    void ShowError(string message, string? title = null);

    /// <summary>
    /// Uyarı mesajı gösterir
    /// </summary>
    /// <param name="message">Gösterilecek uyarı mesajı</param>
    /// <param name="title">Mesaj başlığı (opsiyonel)</param>
    void ShowWarning(string message, string? title = null);

    /// <summary>
    /// Bilgi mesajı gösterir
    /// </summary>
    /// <param name="message">Gösterilecek bilgi mesajı</param>
    /// <param name="title">Mesaj başlığı (opsiyonel)</param>
    void ShowInfo(string message, string? title = null);
}
