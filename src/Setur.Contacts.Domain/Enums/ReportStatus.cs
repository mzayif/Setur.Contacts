using System.ComponentModel;

namespace Setur.Contacts.Domain.Enums;

public enum ReportStatus
{
    /// <summary>
    /// Rapor Hazırlanıyor
    /// </summary>
    [Description("Rapor Hazırlanıyor")]
    Preparing = 1,
    /// <summary>
    /// Rapor Tamamlandı
    /// </summary>
    [Description("Rapor Tamamlandı")]
    Completed = 2,
    /// <summary>
    /// Rapor Başarısız
    /// </summary>
    [Description("Rapor Başarısız")]
    Failed = 3
}
