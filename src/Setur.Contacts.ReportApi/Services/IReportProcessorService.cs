using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.ReportApi.Services;

/// <summary>
/// <b>Report Processor Service Interface</b><br/>
/// <br/>
/// Bu servis, rapor işleme mantığını yönetir. Background service'den gelen rapor isteklerini alır ve işler.<br/>
/// <br/>
/// İş Akışı:<br/>
/// 1. Background service'den rapor isteği alınır<br/>
/// 2. Report status'u "Preparing" yapılır<br/>
/// 3. Rapor türüne göre veri işlenir. İsteğe göre ContactApi'den veya DB'den veri çekilir<br/>
/// 4. İşlenen veriler cache'e kaydedilir<br/>
/// 5. Report status'u "Completed" yapılır<br/>
/// <br/>
/// Tetiklenme:
/// - Background service tarafından tetiklenir
/// - Kafka consumer tarafından tetiklenir (gelecekte)
/// </summary>
public interface IReportProcessorService
{
    /// <summary>
    /// Rapor işleme sürecini başlatır
    /// </summary>
    /// <param name="reportId">İşlenecek rapor ID'si</param>
    /// <param name="reportType">Rapor türü</param>
    /// <param name="parameters">Rapor parametreleri (JSON string)</param>
    /// <returns>İşlem tamamlandığında Task</returns>
    Task ProcessReportAsync(Guid reportId, ReportType reportType, string parameters);
}
