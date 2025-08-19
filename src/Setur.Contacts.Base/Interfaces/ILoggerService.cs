namespace Setur.Contacts.Base.Interfaces;

/// <summary>
/// Uygulama genelinde kullanılacak loglama servisi arayüzü.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Debug seviyesinde log kaydı oluşturur.
    /// </summary>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Information seviyesinde log kaydı oluşturur.
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Warning seviyesinde log kaydı oluşturur.
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Error seviyesinde log kaydı oluşturur.
    /// </summary>
    void LogError(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Fatal seviyesinde log kaydı oluşturur.
    /// </summary>
    void LogFatal(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Belirli bir seviyede log kaydı oluşturur.
    /// </summary>
    void Log(LogLevel level, string message, Exception? exception = null, params object[] args);
}

/// <summary>
/// Log seviyelerini tanımlayan enum.
/// </summary>
public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error,
    Fatal
} 