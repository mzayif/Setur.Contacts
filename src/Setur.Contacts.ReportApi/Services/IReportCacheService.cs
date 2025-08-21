using Setur.Contacts.Domain.Models;

namespace Setur.Contacts.ReportApi.Services;

/// <summary>
/// <b>Report Cache Servisi</b><br/>
/// <br/>
/// Bu servis, rapor verilerinin geçici olarak saklanmasını yönetir.<br/>
/// <br/>
/// İş Akışı:<br/>
/// 1. ReportProcessorService işlenmiş verileri cache'e kaydeder<br/>
/// 2. Kullanıcı rapor detaylarını istediğinde cache'den alınır<br/>
/// 3. Belirtilen süre sonunda otomatik olarak silinir<br/>
/// 4. Kullanıcı "kalıcı kaydet" isterse database'e taşınır<br/>
/// <br/>
/// Kullanım Amacı:<br/>
/// - Hızlı veri erişimi<br/>
/// - Database yükünü azaltma<br/>
/// - Geçici veri saklama<br/>
/// - Performans optimizasyonu<br/>
/// <br/>
/// Cache Stratejisi:<br/>
/// - Redis (production) veya In-Memory (development)<br/>
/// - Süre sınırı Default olarak 24 saat TTL (Time To Live)<br/>
/// - Otomatik temizlik<br/>
/// </summary>
public interface IReportCacheService
{
    /// <summary>
    /// Cache'den rapor verisini getirir
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <returns>Rapor verisi. Bulunamaz ise null</returns>
    Task<ReportCacheData?> GetReportAsync(Guid reportId);

    /// <summary>
    /// Rapor verisini cache'e kaydeder
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <param name="reportData">Kaydedilecek rapor verisi</param>
    /// <returns>Kaydetme işlemi</returns>
    Task SetReportAsync(Guid reportId, ReportCacheData reportData);

    /// <summary>
    /// Rapor verisini cache'den siler
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <returns>Silme başarılı ise true</returns>
    Task<bool> DeleteReportAsync(Guid reportId);

    /// <summary>
    /// Rapor verisinin cache'de olup olmadığını kontrol eder
    /// </summary>
    /// <param name="reportId">Rapor ID'si</param>
    /// <returns>Cache'de varsa true</returns>
    Task<bool> ExistsAsync(Guid reportId);
}
